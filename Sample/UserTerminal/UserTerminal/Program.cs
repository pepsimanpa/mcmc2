using DdsServiceRuntime;
using Omg.Dds.Core;
using Omg.Types.Dynamic;
using Rti.Dds.Domain;
using Rti.RequestReply;
using Rti.Types.Dynamic;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Xml.Linq;
using Types;

namespace UserTerminal
{
    internal class Program
    {
        private static ControlArguments ControlArguments = new();
        private static RuntimeManager? RuntimeManager = null;
        private static List<RequestReplyChannelInfo> Channels = [];
        private static Terminal MainTerminal = new();

        static async Task Main(string[] args)
        {
            InitInternalCommunication();

            MainTerminal.ParseArguments(["-h"]);
            while(true)
            {
                string[] cmd = Console.ReadLine()!.Split(" ");
                var arguments = MainTerminal.ParseArguments(cmd);
                if (arguments is null)
                    return;

                if (arguments is RequestArguments reqArgs)
                {
                    if (reqArgs.RequestControlSpecs)
                    {
                        SendControlSpecListRequest("TSSS", "towedSideScanSonar");
                    }
                }
                else if (arguments is ControlArguments ctrlArgs)
                {
                    if (!string.IsNullOrEmpty(ctrlArgs.Add))
                    {
                        string[] tokens = ctrlArgs.Add.Split(',');
                        if(tokens.Length != 2)
                        {
                            Console.WriteLine("please write {id,value}");
                            continue;
                        }

                        ControlArguments.Params.Add((tokens[0], tokens[1]));
                        Console.WriteLine($"Add({tokens[0]},{tokens[1]})");
                    }

                    if (ctrlArgs.Clear)
                    {
                        ControlArguments.ClearAll();
                        Console.WriteLine("Cleared All Control Settings");
                    }

                    if (!string.IsNullOrEmpty(ctrlArgs.Id))
                    {
                        ControlArguments.Id = ctrlArgs.Id;
                        Console.WriteLine($"Id({ControlArguments.Id})");
                    }

                    if (ctrlArgs.Run)
                    {
                        if (ControlArguments.Id is null)
                        {
                            Console.WriteLine($"id is null");
                            continue;
                        }

                        SendControlExecutionRequest(ControlArguments);
                    }

                    if (ctrlArgs.Show)
                    {
                        Console.WriteLine($"control id: {ControlArguments.Id}");
                        Console.WriteLine($"control params: ");
                        Console.WriteLine(ControlArguments.Params);
                    }
                }
                else
                {
                    if (arguments.Version)
                    {
                        Console.WriteLine("v0.1");
                    }
                }
            }
        }

        private static void InitInternalCommunication()
        {
            Channels.Add(new() { RequestTypeName = "Messages::OperationManagement::ControlSpecListRequest", ReplyTypeName = "Messages::OperationManagement::ControlSpecListReply" });
            Channels.Add(new() { RequestTypeName = "Messages::OperationManagement::ControlExecutionRequest", ReplyTypeName = "Messages::OperationManagement::ControlExecutionReply" });
            Channels.Add(new() { RequestTypeName = "Messages::OperationManagement::MonitorSpecListRequest", ReplyTypeName = "Messages::OperationManagement::MonitorSpecListReply" });

            RuntimeManager = new(@".\\svc\\USER_QOS_PROFILES.xml", "Mcmc2ParticipantLibrary::UserTerminalParticipant");

            // Register Requester
            var controlSpecListRequester = RuntimeManager?.RegisterRequester(Channels[0]);
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await controlSpecListRequester?.WaitForRepliesAsync(1)!;
                    var replies = controlSpecListRequester?.TakeReplies();
                    foreach (var reply in replies?.ValidData()!)
                    {
                        Console.WriteLine($"Reply received: \n{reply}");
                    }
                }
            });

            var controlExecutionRequester = RuntimeManager?.RegisterRequester(Channels[1]);
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await controlExecutionRequester?.WaitForRepliesAsync(1)!;
                    var replies = controlExecutionRequester?.TakeReplies();
                    foreach (var reply in replies?.ValidData()!)
                    {
                        Console.WriteLine($"Reply received: \n{reply}");
                    }
                }
            });
        }

        private static void SendControlSpecListRequest(string targetId, string targetName)
        {
            List<(string, object)> cmd = new();
            cmd.Add(("targetId", targetId));
            cmd.Add(("targetName", targetName));

            var requester = RuntimeManager?.GetRequester(Channels[0]);
            using var sample = RuntimeManager?.CreateSample(Channels[0].RequestTypeName, cmd);

            try
            {
                // Send Request
                requester?.SendRequest(sample!);
                Console.WriteLine("Sent Request: ");
                Console.WriteLine(sample);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout");
            }
        }

        private static void SendControlExecutionRequest(ControlArguments ctrlArgs) 
        {
            var requester = RuntimeManager?.GetRequester(Channels[1]);
            using var sample = new DynamicData(RuntimeManager?.GetDynamicType(Channels[1].RequestTypeName));

            sample.SetAnyValue("targetId", ctrlArgs.Id);
            sample.SetAnyValue("controlSpec.id", ctrlArgs.Id);
            sample.SetAnyValue("controlSpec.name", ctrlArgs.Id);
            sample.SetAnyValue("controlSpec.numsOfParams", ctrlArgs.Params.Count);

            int index = 0;
            foreach(var param in ctrlArgs.Params)
            {
                sample.SetAnyValue($"controlSpec.params[{index}].fieldName", param.Item1);
                sample.SetAnyValue($"controlSpec.params[{index}].fixedValue", param.Item2);
                index++;
            }

            try
            {
                // Send Request
                requester?.SendRequest(sample!);
                Console.WriteLine("Sent Request: ");
                Console.WriteLine(sample);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout");
            }
        }
    }
}
