using System;
using System.Collections.Generic;

using System.Reflection;
[assembly: AssemblyTitle("psburn")] // File Description
[assembly: AssemblyProduct("psburn")] // Product Name
[assembly: AssemblyFileVersion("1.0.0")] // File Version


namespace PsburnPowershellScript
{
    class Utils
    {
        /// <summary>
        /// Runs a subprocess from a executable with args.
        /// <code>> Psburn.Utils.RunSubprocess("cmd", "/c=echo Hello World");</code>
        /// <code>Hello World</code>
        /// </summary>
        /// <param name="file">Executable path</param>
        /// <param name="args">Args to supply</param>
        /// <returns>subprocess exit code</returns>
        public static int RunSubprocess(string file, string args, bool shell = false)
        {
            try
            {
                var Process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = file,
                        Arguments = args,
                        UseShellExecute = shell,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                Process.Start();

                while (!Process.StandardOutput.EndOfStream)
                {
                    var line = Process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                Process.WaitForExit();
                return 0;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        /// <summary>
        /// Prints colored text to console.
        /// <code>> Psburn.Utils.PrintColoredText("caution", ConsoleColor.Yellow);</code>
        /// <code>caution</code>
        /// </summary>
        /// <param name="Text">Text to print</param>
        /// <param name="Color">Color of text</param>
        /// <param name="End">End of line</param>
        public static void PrintColoredText(string Text, ConsoleColor Color, string End = "")
        {
            Console.ForegroundColor = Color;
            Console.Write(Text + End);
            Console.ResetColor();
        }

        /// <summary>
        /// Checks wether platform is windows or not
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                string Platform = Environment.OSVersion.Platform.ToString().ToLower();
                if (Platform.StartsWith("win")) { return (true); }
                else { return (false); }
            }
        }
    }

    class DataParser
    {
        public string[] ParsedArguments;

        /// <summary>
        /// DataParser Constructor
        /// </summary>
        /// <param name="ParsedData">Parsed parameters array</param>
        public DataParser(string[] ParsedData)
        {
            ParsedArguments = ParsedData;
        }

        /// <summary>
        /// Parse positional arguments
        /// </summary>
        /// <returns>Postional arguments array</returns>
        public string[] ParsePostionalArguments()
        {
            List<string> Parameters = new List<string> { };

            foreach (string Argument in ParsedArguments)
            {
                string Variable = Argument.Split(',')[0];
                string Required = Argument.Split(',')[2];

                if (Required == "true") { Parameters.Add(Variable); }
            }
            return Parameters.ToArray();
        }

        /// <summary>
        /// Parse variable types inside a dictionary
        /// </summary>
        /// <returns>Key: Variable, Value: Type</returns>
        public Dictionary<string, string> ParseArgumentsTypesDictionary()
        {
            Dictionary<string, string> ArgumentsTypes = new Dictionary<string, string>();

            foreach (string Argument in ParsedArguments)
            {
                string Variable = Argument.Split(',')[0];
                string Type = Argument.Split(',')[1];
                ArgumentsTypes.Add(Variable, Type);
            }
            return ArgumentsTypes;
        }

        /// <summary>
        /// Parse bool variables values inside a dictionary
        /// </summary>
        /// <returns>Key: Variable, Value: Value</returns>
        public Dictionary<string, string> ParseArgumentsBoolsDictionary()
        {
            Dictionary<string, string> ArgumentsBools = new Dictionary<string, string>();

            foreach (string Argument in ParsedArguments)
            {
                string Variable = Argument.Split(',')[0];
                string Value = Argument.Split(',')[3];
                ArgumentsBools.Add(Variable, Value);
            }
            return ArgumentsBools;
        }

        /// <summary>
        /// Parse variable alias inside a dictionary
        /// </summary>
        /// <returns>Key: Variable, Value: Alias</returns>
        public Dictionary<string, string> ParseArgumentsAliasDictionary()
        {
            Dictionary<string, string> ArgumentsAlias = new Dictionary<string, string>();

            foreach (string Argument in ParsedArguments)
            {
                string Variable = Argument.Split(',')[0];
                string Alias = Argument.Split(',')[4];

                if (Alias == "") { continue; }

                ArgumentsAlias.Add(Variable, Alias);
            }
            return ArgumentsAlias;
        }

        /// <summary>
        /// Parse alias assosiated with which variable inside a dictionary
        /// </summary>
        /// <returns>Key: Alias, Value: Variable</returns>
        public Dictionary<string, string> ParseArgumentsReverseAliasDictionary()
        {
            Dictionary<string, string> ArgumentsReverseAlias = new Dictionary<string, string>();

            foreach (string Argument in ParsedArguments)
            {
                string Variable = Argument.Split(',')[0];
                string Alias = Argument.Split(',')[4];

                if (Alias == "") { continue; }

                ArgumentsReverseAlias.Add(Alias, Variable);
            }
            return ArgumentsReverseAlias;
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            /*
			Progam uses many variables, some variables and their meanings are listed below:
			1) ExPolicy: Execution policy for powershell.
			2) PSScriptFile: Main powershell script need to be runned.
			3) CatFile: Enables cat features.
			4) BlockCat: Blocks cat featues.
			5) PSScriptRoot: Root path of exe and acts as PSScriptRoot.
			6) PSEmbedString: Main finally generated powershell script.
			7) DefaultArgumentsCode: Auto generated default values of argument.
			8) ParsedParameters: Parsed parameters for further parsing.
			9) PSScriptName: Name of exe.
			10) HelpArgumentsTexts: Auto generated help texts.
			11) ProgramDescription: Desciption for program.
            12) AllExamples: Examples related to use program.
			13) ProgramUsage: Usage for program.
			14) CurrentVariable: Upates on each iteration of args and contains variable name.
            15) LoopCount: Counter for args array.
            16) OneDir: Program is going to be hosted in one directory or not.
            17) Executable: Path of powershell executable.

			Special Annotations:
			1) will come: means that data will be pushed here at time of compile.
            2) else removed: line be set as blank if sspecific condition is false.
			*/

            // Main script
            string ExPolicy = "Bypass"; // will come
            string PSScriptFile = "Write-Output 'Hello World!'"; // will come

            // Cat features
            bool CatFile = false;
            bool BlockCat = false; // will come

            // Fixing some lost variables and appending default values to defined variables in powershell script
            string PSScriptRoot = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string PSEmbedString = string.Format("$PSScriptRoot = '{0}'\n\n", PSScriptRoot);

            string[] DefaultArgumentsCode = { }; // will come
            PSEmbedString += "\n";
            foreach (string DefaultCodeLine in DefaultArgumentsCode)
            {
                PSEmbedString += DefaultCodeLine + "\n";
            }

            // Parse parameters from powershell script
            string[] ParsedParameters = { }; // will come
            DataParser ParseParametersData = new DataParser(ParsedParameters);
            string[] PostionalArguments = ParseParametersData.ParsePostionalArguments();
            Dictionary<string, string> ArgumentsType = ParseParametersData.ParseArgumentsTypesDictionary();
            Dictionary<string, string> ArgumentsBools = ParseParametersData.ParseArgumentsBoolsDictionary();
            Dictionary<string, string> ArgumentsAlias = ParseParametersData.ParseArgumentsAliasDictionary();
            Dictionary<string, string> ArgumentsReverseAlias = ParseParametersData.ParseArgumentsReverseAliasDictionary();

            // Program help related texts generations
            string PSScriptName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string FileBaseName = System.IO.Path.GetFileNameWithoutExtension(PSScriptName);

            string ProgramDescription = "dynamically auto compiled embedded powershell command script."; // will come
            ProgramDescription = ProgramDescription.Replace("$newline", "\n");
            string[] HelpArgumentsTexts = { }; // will come
            string[] AllExamples = { }; // will come

            string ProgramUsage = string.Format("usage: {0} [-h] [--cat] ", PSScriptName);

            foreach (string Argument in ArgumentsType.Keys)
            {
                if (ArgumentsAlias.ContainsKey(Argument))
                {
                    if (ArgumentsType[Argument] == "bool") { ProgramUsage += string.Format("[-{0}] ", ArgumentsAlias[Argument]); }
                    else { ProgramUsage += string.Format("[-{0} {1}] ", ArgumentsAlias[Argument], Argument.ToUpper()); }
                }

                else
                {
                    if (ArgumentsType[Argument] == "bool") { ProgramUsage += string.Format("[--{0}] ", Argument); }
                    else { ProgramUsage += string.Format("[--{0} {1}] ", Argument, Argument.ToUpper()); }
                }
            }

            ProgramUsage = ""; // will come, else removed

            // Parsing command line arguments
            string CurrentVariable = "";
            int LoopCount = 0;

            foreach (string Argument in args)
            {
                // -h, --help Handler
                if (Argument == "-h" || Argument == "--help")
                {
                    Console.WriteLine(ProgramUsage + "\n");
                    Console.WriteLine(ProgramDescription + "\n");

                    foreach (string line in HelpArgumentsTexts) { Console.WriteLine(line); }
                    if (HelpArgumentsTexts.Length == 0) { Console.WriteLine("postional arguments:"); }
                    Console.WriteLine(string.Format("  {0,-28} {1}", "--cat", "instead of running cat powershell script into console (default: false)"));
                    Console.WriteLine(string.Format("  {0,-28} {1}\n", "-h, --help", "show this help message and exit"));

                    Console.WriteLine(string.Format("examples:\n\t{0}.exe --cat > {0}.ps1", System.IO.Path.GetFileNameWithoutExtension(PSScriptName)));

                    foreach (string line in AllExamples)
                    {
                        string ExampleLine;
                        ExampleLine = line.Replace("$file_base_name", FileBaseName);
                        ExampleLine = ExampleLine.Replace("$double_qoutes", "\"");

                        Console.WriteLine(string.Format("\t{0}", ExampleLine));
                    }

                    Environment.Exit(0);
                }

                // --cat Handler
                else if (Argument == "--cat") { CatFile = true; }

                // -, -- Handler
                else if (Argument.StartsWith("-"))
                {
                    // Check for alias
                    if (ArgumentsAlias.ContainsValue(Argument.Split('-')[1]))
                    {
                        CurrentVariable = ArgumentsReverseAlias[(Argument.Split('-')[1])];
                    }

                    else
                    {
                        // try to catch variable if failed raise a error
                        try
                        {
                            CurrentVariable = Argument.Split('-')[2]; // raise index error for unrecognized - arguments
                            string Error = ArgumentsType[CurrentVariable]; // raise KeyNotFoundException error for unrecognized -- arguments or anything else
                        }

                        catch
                        {
                            Console.WriteLine(ProgramUsage);
                            Utils.PrintColoredText("error: ", ConsoleColor.Red);
                            Console.WriteLine("unrecognized arguments: " + Argument);
                            Environment.Exit(1);
                        }
                    }

                    // Bool Switch - boolean variable acts as switches by default 
                    if (ArgumentsType[CurrentVariable] == "bool")
                    {
                        if (ArgumentsBools[CurrentVariable] == "false")
                        {
                            PSEmbedString += string.Format("${0} = $true\n", CurrentVariable);
                        }

                        else if (ArgumentsBools[CurrentVariable] == "true")
                        {
                            PSEmbedString += string.Format("${0} = $false\n", CurrentVariable);
                        }
                    }

                    // Conditions to raise errors if no values are supplied
                    else
                    {
                        bool RaiseError = false;

                        try
                        {
                            if (args[LoopCount + 1].StartsWith("-")) { RaiseError = true; }
                        }

                        catch { RaiseError = true; }

                        if (RaiseError)
                        {
                            string ErrorArg = "";

                            if (Argument == ("--" + CurrentVariable)) { ErrorArg = Argument; }
                            else { ErrorArg = Argument + "/--" + CurrentVariable; }

                            Console.WriteLine(ProgramUsage);
                            Utils.PrintColoredText("error: ", ConsoleColor.Red);
                            Console.WriteLine(string.Format("argument {0}: expected one argument", ErrorArg));
                            Environment.Exit(1);
                        }
                    }
                }

                // Variable Handler
                else
                {
                    // String Switch
                    if (ArgumentsType[CurrentVariable] == "string")
                    {
                        PSEmbedString += string.Format("${0} = '{1}'\n", CurrentVariable, Argument);
                    }

                    // Float Switch - works fine for integers too
                    else if (ArgumentsType[CurrentVariable] == "float")
                    {
                        PSEmbedString += string.Format("${0} = [double] '{1}'.Replace('@', '')\n", CurrentVariable, Argument);
                    }
                }

                LoopCount += 1;
            }

            // Checking for unsupplied postional arguments and raising errors
            string CheckAlias = "";

            foreach (string Argument in PostionalArguments)
            {
                if (ArgumentsAlias.ContainsKey(Argument))
                {
                    CheckAlias = ArgumentsAlias[Argument];

                    if (string.Join("", args).Contains("--" + Argument) == false && string.Join("", args).Contains("-" + CheckAlias) == false)
                    {
                        Console.WriteLine(ProgramUsage);
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine(string.Format("the following arguments are required: -{0}/--{1}", CheckAlias, Argument));
                        Environment.Exit(1);
                    }
                }

                else
                {
                    if (string.Join("", args).Contains("--" + Argument) == false)
                    {
                        Console.WriteLine(ProgramUsage);
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine(string.Format("the following arguments are required: --{0}", Argument));
                        Environment.Exit(1);
                    }
                }
            }

            // Final merge of orginal powershell script with genrated parsed variables
            PSEmbedString += "\n" + PSScriptFile;

            // Cat file features if blocked raise errors
            if (CatFile)
            {
                if (BlockCat)
                {
                    Utils.PrintColoredText("error: ", ConsoleColor.Red);
                    Console.WriteLine("cat is blocked during runtime");
                }

                else { Console.WriteLine(PSEmbedString); }

                Environment.Exit(1);
            }

            // Self contained specific checks
            bool OneDir = false; // will come

            // Determining the path of powershell executable
            string Executable = Utils.IsWindows ? "pwsh.exe" : "pwsh";

            if (OneDir)
            {
                Executable = Path.Combine(PSScriptRoot, "pwsh", Executable);
            }

            if (File.Exists(Executable) == false)
            {
                Executable = Utils.IsWindows ? "powershell.exe" : "pwsh";
            }

            // Final call to powershell
            Utils.RunSubprocess(Executable, string.Format("-ExecutionPolicy {0} -Command \"{1}\"", ExPolicy, PSEmbedString));
        }
    }
}
