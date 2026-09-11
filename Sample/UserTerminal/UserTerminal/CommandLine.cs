using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTerminal
{
    public class Terminal
    {
        public Terminal() { }

        public RootArguments? ParseArguments(string[] args)
        {
            // Create a root command with some options
            RootCommand rootCommand = new()
            {
                Description = "Example UMS Operation Management."
            };
            Option<bool> versionOption = new("--version", "-v")
            {
                Description = "Displays version"
            };

            Option<bool> requestControlSpecsOption = new("--request-control-specs", "-rcc")
            {
                Description = "Request Control Specification"
            };
            Command requestCommand = new("request", "request")
            {
                requestControlSpecsOption
            };

            Option<string> addOption = new("--add", "-a")
            {
                Description = "Add Parameter"
            };
            Option<bool> clearOption = new("--clear", "-cl")
            {
                Description = "Clear Parameters"
            };
            Option<string> idOption = new("--id", "-i")
            {
                Description = "Set Control ID"
            };
            Option<bool> runOption = new("--run", "-r")
            {
                Description = "Run Control Command"
            };
            Option<bool> showOption = new("--show", "-sh")
            {
                Description = "Show Settings"
            };
            Command controlCommand = new("control", "control")
            {
                addOption,
                clearOption,
                idOption,
                runOption,
                showOption
            };

            RootArguments rootResult = new();
            rootCommand.Options.Add(versionOption);
            rootCommand.Subcommands.Add(requestCommand);
            rootCommand.Subcommands.Add(controlCommand);
            rootCommand.SetAction(parseResult =>
            {
                rootResult.Version = parseResult.GetValue(versionOption);
            });

            RequestArguments requestResult = new();
            requestCommand.SetAction(parseResult =>
            {
                requestResult.RequestControlSpecs = parseResult.GetValue(requestControlSpecsOption);

                rootResult = requestResult;
            });

            ControlArguments controlArguments = new();
            controlCommand.SetAction(parseResult =>
            {
                controlArguments.Add = parseResult.GetValue(addOption);
                controlArguments.Clear = parseResult.GetValue(clearOption);
                controlArguments.Id = parseResult.GetValue(idOption);
                controlArguments.Run = parseResult.GetValue(runOption);
                controlArguments.Show = parseResult.GetValue(showOption);

                rootResult = controlArguments;
            });

            rootCommand.Parse(args).Invoke();
            if (rootCommand.Parse(args).GetResult(rootCommand.Options[0]) is not null)
                return null;

            return rootResult;
        }
    }

    public record RootArguments
    {
        public bool Version { get; set; }
    }

    public record RequestArguments : RootArguments
    {
        public bool RequestControlSpecs { get; set; }
    }

    public record ControlArguments : RootArguments
    {
        public string? Add { get; set; }
        public bool Clear { get; set; }
        public string? Id { get; set; }
        public bool Run { get; set; }
        public bool Show { get; set; }
        public List<(string, object)> Params { get; set; } = [];
        public void ClearAll()
        {
            Params.Clear();
            Id = "";
        }
    }
}