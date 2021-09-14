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
            RootCommand.AddGlobalOption(new Option<bool>(
                aliases: new string[] { "--verbose" },
                getDefaultValue: () => false,
                description: "generate more outputs and logs than usual")
            );
            
            // Child Command (create)
            var ChildCommandCreate = new Command("create", "create a psburn c# file");
            RootCommand.AddCommand(ChildCommandCreate);

            // Child Command (create) - Arguments
            ChildCommandCreate.AddArgument(new Argument<string>("psscript", "path of powershell script"));

            // Child Command (create) - Options
            ChildCommandCreate.AddOption(new Option<string>(
                aliases: new string[] { "-o", "--output" },
                getDefaultValue: () => "working directory",
                description: "path of output c# file")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--py" },
                getDefaultValue: () => false,
                description: "generates python code")
            );


            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--self-contained" },
                getDefaultValue: () => false,
                description: "enable this option if you are using --powershell-zip")
            );


            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--embed-resources" },
                getDefaultValue: () => false,
                description: "enable this option if you are using --resources-zip")
            );
            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
            );

            ChildCommandCreate.AddOption(new Option<string>(
                aliases: new string[] { "--execution-policy" },
                getDefaultValue: () => "Bypass",
                description: "execution policy for the powershell script")
            );

            ChildCommandCreate.AddOption(new Option<bool>(
                aliases: new string[] { "--blockcat" },
                getDefaultValue: () => false,
                description: "block cating of script which prevents script exposure")
            );

            ChildCommandCreate.Handler = CommandHandler.Create<string, string, bool, bool, bool, bool, string, bool, bool>(PsburnCommands.Create);

            // Child Command (build)
            var ChildCommandBuild = new Command("build", "build an executable from c# program");
            RootCommand.AddCommand(ChildCommandBuild);

            // Child Command (build) - Arguments
            ChildCommandBuild.AddArgument(new Argument<string>("psscript", "path of powershell script"));
            ChildCommandBuild.AddArgument(new Argument<string>("binderfile", "path of binding py or c# file"));

            // Child Command (build) - Options
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

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--onedir" },
                getDefaultValue: () => false,
                description: "run powershell from current directory")
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

            ChildCommandBuild.AddOption(new Option<string>(
                aliases: new string[] { "--cscpath" },
                getDefaultValue: () => "auto detect",
                description: "c# compiler path (C:\\Windows\\Microsoft.Net\\Framework\\<version>\\csc.exe)")
            );

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "--pyinstaller-prompt" },
                getDefaultValue: () => false,
                description: "ask for pyinstaller")
            );

            ChildCommandBuild.AddOption(new Option<bool>(
                aliases: new string[] { "-d", "--debug" },
                getDefaultValue: () => false,
                description: "don't delete runtime generated files")
            );

            ChildCommandBuild.Handler = CommandHandler.Create<string, string, string, string, bool, string, bool, bool, string, bool, bool, bool>(PsburnCommands.Build);

            return RootCommand.InvokeAsync(args).Result;
        }
    }
}
