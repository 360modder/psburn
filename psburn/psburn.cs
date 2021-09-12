using System.CommandLine;
using System.CommandLine.Invocation;


namespace Psburn
{
    class Psburn
    {
        public static int Main(string[] args)
        {
            // Root Command (psburn)
            var RootCommand = new RootCommand("psburn is a tool to package powershell scripts into executables " +
                                              "by encapsulating it inside a c# or python program.");

            // Global Options
            RootCommand.AddGlobalOption(new Option<string>(
                aliases: new string[] { "-o", "--output" },
                getDefaultValue: () => "working directory",
                description: "path of output files")
            );

            RootCommand.AddGlobalOption(new Option<bool>(
                aliases: new string[] { "-v", "--verbose" },
                getDefaultValue: () => false,
                description: "generate more outputs and logs than usual")
            );

            // Child Command (create)
            var ChildCommandCreate = new Command("create", "create a psburn c# file");
            RootCommand.AddCommand(ChildCommandCreate);

            // Child Command (create) - Arguments
            ChildCommandCreate.AddArgument(new Argument<string>("psscript", "path of powershell script"));

            // Child Command (create) - Options
            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "-e", "--experimental" },
                getDefaultValue: () => false,
                description: "use experimental features to create script")
            );

            ChildCommandCreate.AddOption(new Option<string>(
                aliases: new string[] { "--execution-policy" },
                getDefaultValue: () => "Bypass",
                description: "execution policy for the powershell script")
            );

            ChildCommandCreate.AddOption(new Option<string>(
                aliases: new string[] { "--script-name" },
                getDefaultValue: () => "basename of input file",
                description: "set name of script")
            );

            ChildCommandCreate.AddOption(new Option<string>(
                aliases: new string[] { "--script-version" },
                getDefaultValue: () => "1.0.0",
                description: "set version of script")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--blockcat" },
                getDefaultValue: () => false,
                description: "block cating of script which prevents script exposure")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--self-contained" },
                getDefaultValue: () => false,
                description: "enable this option if you are using --powershell-zip")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--embed-resources" },
                getDefaultValue: () => false,
                description: "enable this option if you are using --resources-zip")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--no-extract" },
                getDefaultValue: () => false,
                description: "this option allows to host scripts purely from program instead of extracted version of them")
            );

            ChildCommandCreate.Handler = CommandHandler.Create<string, bool, string, string, string, bool, bool, bool, bool, bool, string, bool>(PsburnCommand.Create);

            // Child Command (cross)
            var ChildCommandCross = new Command("cross", "create a psburn py script");
            RootCommand.AddCommand(ChildCommandCross);

            // Child Command (cross) - Arguments
            ChildCommandCross.AddArgument(new Argument<string>("psscript", "path of powershell script"));

            // Child Command (cross) - Options
            ChildCommandCross.AddOption(new Option<string>(
                aliases: new string[] { "--execution-policy" },
                getDefaultValue: () => "Bypass",
                description: "execution policy for the powershell script")
            );

            ChildCommandCross.AddOption(new Option<bool>(
                aliases: new string[] { "--blockcat" },
                getDefaultValue: () => false,
                description: "block cating of script which prevents script exposure")
            );

            ChildCommandCross.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
            );

            ChildCommandCross.Handler = CommandHandler.Create<string, string, bool, bool, string, bool>(PsburnCommand.Cross);

            // Child Command (build)
            var ChildCommandBuild = new Command("build", "build an executable from c# program");
            RootCommand.AddCommand(ChildCommandBuild);

            // Child Command (build) - Arguments
            ChildCommandBuild.AddArgument(new Argument<string>("csfile", "path of c# file"));
            ChildCommandBuild.AddArgument(new Argument<string>("psscript", "path of powershell script"));

            // Child Command (build) - Options
            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "-d", "--debug" },
                getDefaultValue: () => false,
                description: "don't delete runtime generated files")
            );

            ChildCommandBuild.AddOption(new Option<string>(
                aliases: new string[] { "-p", "--powershell-zip" },
                getDefaultValue: () => "no zip",
                description: "create a self contained executable")
            );

            ChildCommandBuild.AddOption(new Option<string>(
                aliases: new string[] { "-r", "--resources-zip" },
                getDefaultValue: () => "no zip",
                description: "embed resources from a zip file")
            );

            ChildCommandBuild.AddOption(new Option<string>(
                aliases: new string[] { "--cscpath" },
                getDefaultValue: () => "auto detect",
                description: "c# compiler path (C:\\Windows\\Microsoft.Net\\Framework\\<version>\\csc.exe)")
            );

            ChildCommandBuild.AddOption(new Option<string>(
                aliases: new string[] { "--icon" },
                getDefaultValue: () => "no icon",
                description: "apply icon to generated executable")
            );

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--no-console" },
                getDefaultValue: () => false,
                description: "create executable without a console, this helps for running scripts in background for gui programs")
            );

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--uac-admin" },
                getDefaultValue: () => false,
                description: "request elevation upon application restart (windows specific)")
            );

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
            );

            ChildCommandBuild.Handler = CommandHandler.Create<string, string, bool, string, string, string, string, bool, bool, bool, string, bool>(PsburnCommand.Build);

            // Child Command (cbuild)
            var ChildCommandCBuild = new Command("cbuild", "build an executable from python script");
            RootCommand.AddCommand(ChildCommandCBuild);

            // Child Command (cbuild) - Arguments
            ChildCommandCBuild.AddArgument(new Argument<string>("pyscript", "path of python script"));
            ChildCommandCBuild.AddArgument(new Argument<string>("psscript", "path of powershell script"));

            // Child Command (cbuild) - Options
            ChildCommandCBuild.AddOption(new Option<bool>(
                aliases: new string[] { "-d", "--debug" },
                getDefaultValue: () => false,
                description: "don't delete runtime generated files")
            );

            ChildCommandCBuild.AddOption(new Option<string>(
                aliases: new string[] { "-p", "--powershell-zip" },
                getDefaultValue: () => "no zip",
                description: "create a self contained executable")
            );

            ChildCommandCBuild.AddOption(new Option<string>(
                aliases: new string[] { "-r", "--resources-zip" },
                getDefaultValue: () => "no zip",
                description: "embed resources from a zip file")
            );

            ChildCommandCBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
            );

            ChildCommandCBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--no-prompt" },
                getDefaultValue: () => false,
                description: "don't ask for any prompts")
            );

            ChildCommandCBuild.Handler = CommandHandler.Create<string, string, bool, string, string, bool, bool, string, bool>(PsburnCommand.CBuild);

            return RootCommand.InvokeAsync(args).Result;
        }
    }
}
