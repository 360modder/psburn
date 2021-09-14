using System;
using System.IO;
using System.Collections.Generic;


namespace PsburnCliParser
{
    internal class DataParser
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

    public class ArgumentParser
    {
        public string selfName;
        public string selfDescription;
        public string[] selfargs;
        public string[] selfParsedParameters;
        public string[] PostionalArguments;
        public Dictionary<string, string> ArgumentsType;
        public Dictionary<string, string> ArgumentsBools;
        public Dictionary<string, string> ArgumentsAlias;
        public Dictionary<string, string> ArgumentsReverseAlias;

        /// <summary>
        /// ArgumentParser Constructor
        /// </summary>
        public ArgumentParser(string Name, string Description, string[] args, string[] ParsedParameters)
        {
            selfName = Name;
            selfDescription = Description;
            selfargs = args;
            selfParsedParameters = ParsedParameters;

            DataParser ParseParametersData = new DataParser(ParsedParameters);
            PostionalArguments = ParseParametersData.ParsePostionalArguments();
            ArgumentsType = ParseParametersData.ParseArgumentsTypesDictionary();
            ArgumentsBools = ParseParametersData.ParseArgumentsBoolsDictionary();
            ArgumentsAlias = ParseParametersData.ParseArgumentsAliasDictionary();
            ArgumentsReverseAlias = ParseParametersData.ParseArgumentsReverseAliasDictionary();
        }

        internal string ParseUsage()
        {
            string ProgramUsage = string.Format("usage: {0} [-h] [--cat] ", selfName.Replace(".exe", ""));

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

            return ProgramUsage;
        }

        public void DoCat(string PSScriptFile, bool BlockCat)
        {
            foreach (string Argument in selfargs)
            {
                if (Argument == "--cat")
                {
                    if (BlockCat)
                    {
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine("cat is blocked during runtime");
                        Environment.Exit(1);
                    }

                    else
                    {
                        Console.WriteLine(PSScriptFile);
                        Environment.Exit(0);
                    }
                }
            }
        }

        public void DoHelp(string[] HelpArgumentsTexts, string[] AllExamples)
        {
            string ProgramUsage = ParseUsage();
            selfDescription = selfDescription.Replace("$newline", "\n");

            foreach (string Argument in selfargs)
            {
                if (Argument == "-h" || Argument == "--help")
                {
                    Console.WriteLine(ProgramUsage + "\n");
                    Console.WriteLine(selfDescription + "\n");

                    foreach (string line in HelpArgumentsTexts) { Console.WriteLine(line); }
                    if (HelpArgumentsTexts.Length == 0) { Console.WriteLine("postional arguments:"); }
                    Console.WriteLine(string.Format("  {0,-28} {1}", "--cat", "instead of running cat powershell script into console (default: false)"));
                    Console.Write(string.Format("  {0,-28} {1}\n", "-h, --help", "show this help message and exit"));

                    if (AllExamples.Length != 0) { Console.WriteLine("\nexamples:"); }
                    foreach (string line in AllExamples)
                    {
                        string ExampleLine;
                        ExampleLine = line.Replace("$file_base_name", selfName);
                        ExampleLine = ExampleLine.Replace("$double_qoutes", "\"");

                        Console.WriteLine(string.Format("\t{0}", ExampleLine));
                    }

                    Environment.Exit(0);
                }
            }
        }
        public string ParseArgs()
        {
            // Parsing command line arguments
            string ProgramUsage = ParseUsage();
            string PSEmbedString = "";
            string CurrentVariable = "";
            int LoopCount = 0;
            
            foreach (string Argument in selfargs)
            {
                // -, -- Handler
                if (Argument.StartsWith("-"))
                {
                    // Check for alias
                    if (ArgumentsAlias.ContainsValue(Argument.Split('-')[1]))
                    {
                        CurrentVariable = ArgumentsReverseAlias[(Argument.Split('-')[1])];
                    }

                    else
                    {
                        try
                        {
                            CurrentVariable = Argument.Split('-')[2];
                            string Error = ArgumentsType[CurrentVariable]; // raises KeyNotFoundException for unrecognized arguments
                        }

                        catch
                        {
                            Console.WriteLine(ProgramUsage);
                            Utils.PrintColoredText("error: ", ConsoleColor.Red);
                            Console.WriteLine("unrecognized arguments: " + Argument);
                            Environment.Exit(1);
                        }
                    }

                    // Bool Switch - boolean variable acts as an switch by default 
                    if (ArgumentsType[CurrentVariable] == "bool")
                    {
                        PSEmbedString += string.Format("${0} = ${1}\n", CurrentVariable, ArgumentsBools[CurrentVariable] == "false");
                    }

                    // Conditions to raise errors if no values are supplied
                    else
                    {
                        bool RaiseError = false;

                        try
                        {
                            if (selfargs[LoopCount + 1].StartsWith("-")) { RaiseError = true; }
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

                    // Float Switch
                    else if (ArgumentsType[CurrentVariable] == "float")
                    {
                        PSEmbedString += string.Format("${0} = [double] '{1}'.Replace('@', '')\n", CurrentVariable, Argument);
                    }
                }

                LoopCount += 1;
            }

            // Checking for unsupplied postional arguments and raising errors
            string CheckAlias;

            foreach (string Argument in PostionalArguments)
            {
                if (ArgumentsAlias.ContainsKey(Argument))
                {
                    CheckAlias = ArgumentsAlias[Argument];

                    if (string.Join("", selfargs).Contains("--" + Argument) == false && string.Join("", selfargs).Contains("-" + CheckAlias) == false)
                    {
                        Console.WriteLine(ProgramUsage);
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine(string.Format("the following arguments are required: -{0}/--{1}", CheckAlias, Argument));
                        Environment.Exit(1);
                    }
                }

                else
                {
                    if (string.Join("", selfargs).Contains("--" + Argument) == false)
                    {
                        Console.WriteLine(ProgramUsage);
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine(string.Format("the following arguments are required: --{0}", Argument));
                        Environment.Exit(1);
                    }
                }
            }

            return PSEmbedString;
        }

        public static string ParseDefaultArgs(string[] DefaultArgumentsCode)
        {
            string PSEmbedString = "\n";
            foreach (string DefaultCodeLine in DefaultArgumentsCode)
            {
                PSEmbedString += DefaultCodeLine + "\n";
            }
            return PSEmbedString;
        }
    }
 }
