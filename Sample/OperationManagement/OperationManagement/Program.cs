using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Specification;
using System.Linq;
using Specification.Node;
using Specification.Semantic.DTO;
using MessageAdapter;
using Rti.Dds.Domain;
using DdsServiceRuntime;
using Rti.Types.Dynamic;
using Types;
using Rti.Dds.Subscription;
using Rti.RequestReply;
using Specification.Binding.DTO;
using Common;

namespace OperationManager
{
    public enum PlatformIDKey
    {
        USV1 = 0,
        USV2
    }

    public enum EquipmentIDKey
    {
        SSS = 0,
        EOTS,
        CAM,
    }

    public record RuntimeContext
    {
        public PlatformIDKey PlatformID;
        public EquipmentIDKey EquipmentID;
        public System.DateTime TimeStamp;
    }

    class Program
    {
        static void Main(string[] args)
        {
            InitVehicleRegistry();
            InitVehicleTransportAdapters(333);
            InitInternalCommunication();

            /// UMS 운용관리
            ///  - Semantic, Binding Spec 다운로드 from 자원관리 CSC
            ///  - Effective Spec Registry 생성
            ///  - Vehicle Instance Repository 초기화
            ///  - 
            ///  - 
            ///  
            /// UMS 연동
            ///  - PulginManager
            ///  - USV 송신 Task --> DDS Service로 대체
            ///  - USV 수신 Task --> DDS Service로 대체
            ///  - AUV 송신 Task
            ///  - AUV 수신 Task
            ///  - MDV 송신 Task
            ///  - MDV 수신 Task
            ///  - EMDW 송신 Task
            ///  - EMDW 수신 Task
            ///  

            while (true) ;
        }

        private static void InitVehicleRegistry()
        {
            VehicleRegistry = new VehicleRegistry(@".\spec\UsvSpecification.xml");
            const string sensorName = "towedSideScanSonar";
            const string filter = "control";
            SensorSpec towedSideScanSonar = VehicleRegistry.Sensors[sensorName];

            towedSideScanSonar.PrintAll(filter);
            Console.WriteLine(new string('=', 160));

            var controls = towedSideScanSonar.SemanticSpecs
                .Where(k => k.Value.ParentId == filter)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            Console.WriteLine($"Namespace: {sensorName}.{filter}");
            Console.WriteLine($"Usage: {{semantic_full_id}} {{param}}");
            Console.WriteLine($"Example: \"{{semantic_full_id}} --semantic\" or \"{{semantic_full_id}} --binding\"");
        }

        public static void InitVehicleTransportAdapters(int ddsDomainId)
        {
            Participant = DomainParticipantFactory.Instance.CreateParticipant(ddsDomainId);
            TransportAdapterRegistry.Register("TowedSonarArrayStartControlType", new TowedSonarArrayStartControlTypeAdapter(Participant, "TowedSonarArrayStartControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyAutoLaunchControlType", new TowedSonarAssemblyAutoLaunchControlTypeAdapter(Participant, "TowedSonarAssemblyAutoLaunchControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyPlatformPowerControlType", new TowedSonarAssemblyPlatformPowerControlTypeAdapter(Participant, "TowedSonarAssemblyPlatformPowerControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyModeControlType", new TowedSonarAssemblyModeControlTypeAdapter(Participant, "TowedSonarAssemblyModeControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyRestartControlType", new TowedSonarAssemblyRestartControlTypeAdapter(Participant, "TowedSonarAssemblyRestartControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyEmergencyStopControlType", new TowedSonarAssemblyEmergencyStopControlTypeAdapter(Participant, "TowedSonarAssemblyEmergencyStopControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyScreanChangeConfigType", new TowedSonarAssemblyScreanChangeConfigTypeAdapter(Participant, "TowedSonarAssemblyScreanChangeConfigType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyManualUltraShortBaseLineMotorControlType", new TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeAdapter(Participant, "TowedSonarAssemblyManualUltraShortBaseLineMotorControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyUltraShortBaseLineStartControlType", new TowedSonarAssemblyUltraShortBaseLineStartControlTypeAdapter(Participant, "TowedSonarAssemblyUltraShortBaseLineStartControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType", new TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeAdapter(Participant, "TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType"));
            TransportAdapterRegistry.Register("TowedSonarAssemblyLaunchAndRecoverySlideStartControlType", new TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeAdapter(Participant, "TowedSonarAssemblyLaunchAndRecoverySlideStartControlType"));
        }

        private static void InitInternalCommunication()
        {
            // Identify Channel Info
            RequestReplyChannelInfo[] channels = {
                new(){RequestTypeName = "Messages::OperationManagement::ControlSpecListRequest", ReplyTypeName = "Messages::OperationManagement::ControlSpecListReply"},
                new(){RequestTypeName = "Messages::OperationManagement::ControlExecutionRequest", ReplyTypeName = "Messages::OperationManagement::ControlExecutionReply" },
                new(){RequestTypeName = "Messages::OperationManagement::MonitorSpecListRequest", ReplyTypeName = "Messages::OperationManagement::MonitorSpecListReply" }
            };

            RuntimeManager = new(@".\\svc\\USER_QOS_PROFILES.xml", "Mcmc2ParticipantLibrary::OperationManagementParticipant");

            // Register ControlSpecListReplier
            var controlSpecReplier = RuntimeManager?.RegisterReplier(channels[0]);
            controlSpecReplier!.RequestsAvailable += (_ =>
            {
                using var requests = _.TakeRequests();
                foreach (var request in requests)
                {
                    if (request.Info.ValidData)
                    {
                        Console.WriteLine("Receive Request: ");
                        Console.WriteLine(request.Data);

                        var replyParam = MakeControlSpecListReply(request.Data);
                        using var reply = RuntimeManager!.CreateSample(channels[0].ReplyTypeName, replyParam);
                        _.SendReply(reply, request.Info);
                    }
                }
            });

            // Register ControlExecutionReplier
            var controlExecutionReplier = RuntimeManager?.RegisterReplier(channels[1]);
            controlExecutionReplier!.RequestsAvailable += (replier =>
            {
                _ = Task.Run(async () =>
                {
                    // Warning State Mutation 
                    using var requests = replier.TakeRequests();
                    foreach (var request in requests)
                    {
                        if (request.Info.ValidData)
                        {
                            Console.WriteLine("Receive Request: ");
                            Console.WriteLine(request.Data);

                            var replyParam = await MakeControlExecutionReply(request.Data);
                            using var reply = RuntimeManager!.CreateSample(channels[1].ReplyTypeName, replyParam);
                            replier.SendReply(reply, request.Info);
                        }
                    }
                });
            });

            // Register MonitorSpecReplier
            var monitorSpecReplier = RuntimeManager?.RegisterReplier(channels[2]);
            monitorSpecReplier!.RequestsAvailable += (_ =>
            {
                using var requests = _.TakeRequests();
                foreach (var request in requests)
                {
                    if (request.Info.ValidData)
                    {
                        Console.WriteLine("Receive Request: ");
                        Console.WriteLine(request.Data);

                        DynamicData reply = new DynamicData(RuntimeManager!.GetDynamicType(channels[2].ReplyTypeName));
                        reply.SetValue("targetId", request.Data.GetValue<string>("targetId"));
                        _.SendReply(reply, request.Info);
                    }
                }
            });
        }

        private static List<(string, object)> MakeControlSpecListReply(DynamicData request)
        {
            const string filter = "control";
            SensorSpec towedSideScanSonar = VehicleRegistry!.Sensors[request.GetValue<string>("targetName")];

            var controls = towedSideScanSonar.SemanticSpecs
                .Where(k => k.Value.ParentId == filter)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            List<(string, object)> result = new();

            result.Add(("targetId", request.GetValue<string>("targetId")));
            result.Add(("targetName", request.GetValue<string>("targetName")));
            result.Add(("numsOfControlSpecs", controls.Count));

            int controlSpecIndex = 0;
            foreach (var controlPair in controls)
            {
                result.Add(($"controlSpecList[{controlSpecIndex}].id", controlPair.Value.Id));
                result.Add(($"controlSpecList[{controlSpecIndex}].cdm", controlPair.Value.Cdm));

                var control = controlPair.Value as ControlSpecNode;
                if (control is null || control.Binding is null)
                {
                    controlSpecIndex++;
                    continue;
                }

                int paramIndex = 0;
                foreach(var field in control.Binding.Fields)
                {
                    result.Add(($"controlSpecList[{controlSpecIndex}].params[{paramIndex}].fieldName", field.name));

                    if(field is FixedFieldDTO fixedField)
                        result.Add(($"controlSpecList[{controlSpecIndex}].params[{paramIndex}].fixedValue", fixedField.value));

                    paramIndex++;
                }

                controlSpecIndex++;
            }

            return result;
        }

        private static async Task<List<(string, object)>> MakeControlExecutionReply(DynamicData request)
        {
            string controlId = request.GetValue<string>("controlSpec.id");
            int numsOfParams = request.GetValue<int>("controlSpec.numsOfParams");
            Dictionary<string, object> @params = [];

            for(int i = 0; i < numsOfParams; i++)
            {
                string fieldName = request.GetValue<string>($"controlSpec.params[{i}].fieldName");
                string value = request.GetValue<string>($"controlSpec.params[{i}].fixedValue");
                @params.Add(fieldName, value);
            }

            SensorSpec towedSideScanSonar = VehicleRegistry!.Sensors["towedSideScanSonar"];
            var controlSpec = towedSideScanSonar.FindSpec(controlId) as ControlSpecNode;
            
            bool isSuccess = controlSpec!.Invoke(@params, TransportAdapterRegistry);
            OperationState report = OperationState.Failed;
            if (isSuccess)
            {
                Console.WriteLine("Execution Success");
                report = OperationState.Finished;
            }
            else
            {
                Console.WriteLine("Execution Failed");
            }

            // Send To Vehicle and Fill the report!
            List<(string, object)> result = [];
            result.Add(("targetId", controlId));
            result.Add(("executionReport", report));

            return result;
        }

        private static RuntimeManager? RuntimeManager = null;
        public static TransportAdapterRegistry TransportAdapterRegistry = new();
        public static DomainParticipant? Participant;
        public static VehicleRegistry? VehicleRegistry;
    }
}
