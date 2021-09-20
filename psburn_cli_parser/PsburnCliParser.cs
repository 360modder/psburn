using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


namespace PsburnCliParser
{
    public class ArgumentDataNamespace
    {
        public string Argument { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Value { get; set; }
        public string Alias { get; set; }
        public string Help { get; set; }

        public ArgumentDataNamespace(string ParsedArgumentData)
        {
            Argument = ParsedArgumentData.Split(',')[0];
            Type = ParsedArgumentData.Split(',')[1];
            Required = ParsedArgumentData.Split(',')[2] == "true";
            Value = ParsedArgumentData.Split(',')[3];
            Alias = ParsedArgumentData.Split(',')[4];
            Help = ParsedArgumentData.Split(',')[5];
        }
    }

    public class ArgumentParser
    {
        public string PName;
        public string PDescription;
        public List<string>AddedArguments = new List<string>();

        public ArgumentParser(string Name = "", string Description = "")
        {
            PName = Name == "" ? Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) : Name;
            PDescription = Description;
        }

        public void AddArgument(string Argument, string Default = "", string Type = "string", string Help = "", string Alias = "", bool Required = false)
        {
            Argument = Argument.Replace("-", "");
            Alias = Alias.Replace("-", "");
            string NRequired = Required ? "true" : "false";
            AddedArguments.Add($"{Argument},{Type},{NRequired},{Default},{Alias},{Help}");
        }

        public void AddArgumentFromParsedParameters(string[] ParsedParameters)
        {
            AddedArguments.AddRange(ParsedParameters);
        }

        internal void SortAddedArguments()
        {
            List<string> TempArgsList = new List<string>();
            ArgumentDataNamespace Arg;

            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);
                if (Arg.Required) { TempArgsList.Add(ArgumentData); }

            }

            foreach (string ParsedArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ParsedArgumentData);
                if (!Arg.Required) { TempArgsList.Add(ParsedArgumentData); }

            }

            AddedArguments.Clear();
            AddedArguments.AddRange(TempArgsList);
        }

        internal string GenerateUsage()
        {
            ArgumentDataNamespace Arg;
            string UsageBlock;
            List<string> ProgramUsage = new List<string>();

            ProgramUsage.Add($"usage: {PName.Replace(".exe", "")} [-h]");

            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);

                if (!Arg.Required)
                {
                    UsageBlock = Arg.Alias.Equals("") ? "--" + Arg.Argument : "-" + Arg.Alias;
                    if (Arg.Type == "bool") { ProgramUsage.Add($"[{UsageBlock}]"); }
                    else { ProgramUsage.Add($"[{UsageBlock} {Arg.Argument.ToUpper()}]"); }
                }
            }

            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);
                if (Arg.Required) { ProgramUsage.Add(Arg.Argument); }
            }

            return string.Join(" ", ProgramUsage);
        }

        public void DisplayHelp(string[] args)
        {
            ArgumentDataNamespace Arg;
            List<string> OutBlock = new List<string>();
            string MoveHelpTextToNewline;
            string OutLine;
            int PostionalCount = 0;
            int OptionalCount = 0;

            foreach (string Argument in args)
            {
                if (Argument == "-h" || Argument == "--help")
                {
                    Console.WriteLine(GenerateUsage() + "\n");
                    Console.WriteLine(PDescription);

                    foreach (string ArgumentData in AddedArguments)
                    {
                        Arg = new ArgumentDataNamespace(ArgumentData);

                        if (Arg.Required && PostionalCount == 0)
                        {
                            Console.WriteLine("\npostional arguments:");
                            PostionalCount += 1;
                        }

                        if (!Arg.Required && OptionalCount == 0)
                        {
                            Console.WriteLine("\noptional arguments:");
                            OptionalCount += 1;
                        }

                        OutBlock.Add(" ");
                        if (Arg.Required) { OutBlock.Add(Arg.Argument); }
                        else
                        {
                            if (Arg.Alias != "") { OutBlock.Add($"-{Arg.Alias},"); }
                            OutBlock.Add($"--{Arg.Argument}");
                            if (Arg.Type != "bool") { OutBlock.Add($"<{Arg.Argument.ToUpper()}>"); }
                        }

                        OutLine = string.Join(" ", OutBlock);
                        MoveHelpTextToNewline = Arg.Help.Equals("") ? "" : @"\n";
                        OutBlock.Clear();

                        if (30 <= OutLine.Length)
                        {
                            OutBlock.Add(OutLine + MoveHelpTextToNewline + string.Format("{0,-30} {1}", "", Arg.Help));
                        }

                        else
                        {
                            OutBlock.Add(string.Format("{0,-30} {1}", OutLine, Arg.Help));
                        }

                        if (!Arg.Required) { OutBlock.Add($"[default: {Arg.Value}]"); }

                        Console.WriteLine(string.Join(" ", OutBlock));
                        OutBlock.Clear();
                    }

                    Console.Write(string.Format("  {0,-28} {1}\n", "-h, --help", "show this help message and exit"));
                    Environment.Exit(0);
                }
            }
        }

        internal void RaiseExpectedOneArgumentError(string Argument)
        {
            Console.WriteLine(GenerateUsage());
            Console.WriteLine($"error: argument {Argument}: expected one argument");
            Environment.Exit(1);
        }

        internal void RaiseArgumentRequiredError(string Argument)
        {
            Console.WriteLine(GenerateUsage());
            Console.WriteLine($"error: the following arguments are required: {Argument}");
            Environment.Exit(1);
        }

        internal void SearchArgumentsValues(ArgumentDataNamespace Arg, Dictionary<string, object> NewParsedValues, string[] args, bool CheckAlias = false)
        {
            string NewValue;

            if (NewParsedValues.ContainsKey(Arg.Argument)) { NewParsedValues.Remove(Arg.Argument); }

            try
            {
                if (Arg.Type == "bool")
                {
                    if (CheckAlias)
                    {
                        if (args[Array.FindIndex(args, element => element.StartsWith("-" + Arg.Alias))].Contains("="))
                        {
                            NewValue = args[Array.FindIndex(args, element => element.StartsWith("-" + Arg.Alias))].Split('=')[1];
                            NewParsedValues.Add(Arg.Argument, NewValue == "true" ? true : false);
                        }

                        else { NewParsedValues.Add(Arg.Argument, Arg.Value == "true" ? false : true); }
                    }

                    else
                    {
                        if (args[Array.FindIndex(args, element => element.StartsWith("--" + Arg.Argument))].Contains("="))
                        {
                            NewValue = args[Array.FindIndex(args, element => element.StartsWith("--" + Arg.Argument))].Split('=')[1];
                            NewParsedValues.Add(Arg.Argument, NewValue == "true" ? true : false);
                        }

                        else { NewParsedValues.Add(Arg.Argument, Arg.Value == "true" ? false : true); }
                    }
                }

                else
                {
                    if (CheckAlias)
                    {
                        if (args[Array.FindIndex(args, element => element.StartsWith("-" + Arg.Alias))].Contains("="))
                        {
                            NewValue = args[Array.FindIndex(args, element => element.StartsWith("-" + Arg.Alias))].Split('=')[1];
                        }

                        else { NewValue = args[Array.FindIndex(args, element => element == "-" + Arg.Alias) + 1]; }
                    }

                    else
                    {
                        if (args[Array.FindIndex(args, element => element.StartsWith("--" + Arg.Argument))].Contains("="))
                        {
                            NewValue = args[Array.FindIndex(args, element => element.StartsWith("--" + Arg.Argument))].Split('=')[1];
                        }

                        else { NewValue = args[Array.FindIndex(args, element => element == "--" + Arg.Argument) + 1]; }
                    }

                    if (NewValue.StartsWith("-")) { RaiseExpectedOneArgumentError(Arg.Argument); }

                    if (Arg.Type == "double") { NewParsedValues.Add(Arg.Argument, Convert.ToDouble(NewValue)); }
                    else { NewParsedValues.Add(Arg.Argument, NewValue); }
                }
            }

            catch (IndexOutOfRangeException) { RaiseExpectedOneArgumentError(Arg.Argument); }
        }

        internal void CheckUnrecognizedArguments(string[] args, Dictionary<string, object> NewParsedValues)
        {
            ArgumentDataNamespace Arg;
            List<string> DefinedArgumentsList = new List<string>();

            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);
                DefinedArgumentsList.Add("--" + Arg.Argument);
                if (Arg.Alias != "") { DefinedArgumentsList.Add("-" + Arg.Alias); }
            }

            foreach (string Argument in args)
            {
                if (!Array.Exists(DefinedArgumentsList.ToArray(), element => element.Equals(Argument.Contains("=") ? Argument.Split('=')[0] : Argument)) && Argument.StartsWith("-"))
                {
                    Console.WriteLine(GenerateUsage());
                    Console.WriteLine($"error: unrecognized arguments: {Argument}");
                    Environment.Exit(1);
                }
            }
        }

        internal void ParsePostionalArguments(string[] args, Dictionary<string, object> NewParsedValues)
        {
            ArgumentDataNamespace Arg;
            int NextPostionalArgumentIndex = 0;
            int NextReversePostionalArgumentIndex = args.Length - 1;

            if (string.Join("", args).StartsWith("-"))
            {
                for (int i = AddedArguments.ToArray().Length; i-- > 0;)
                {
                    Arg = new ArgumentDataNamespace(AddedArguments[i]);

                    if (Arg.Required)
                    {
                        try
                        {
                            if (args[NextReversePostionalArgumentIndex].StartsWith("-")) { RaiseArgumentRequiredError(Arg.Argument); }
                            NewParsedValues.Add(Arg.Argument, args[NextReversePostionalArgumentIndex]);
                            NextReversePostionalArgumentIndex -= 1;
                        }
                        catch (IndexOutOfRangeException) { RaiseArgumentRequiredError(Arg.Argument); }
                    }

                }
            }

            else
            {
                foreach (string ArgumentData in AddedArguments)
                {
                    Arg = new ArgumentDataNamespace(ArgumentData);

                    if (Arg.Required)
                    {
                        try
                        {
                            if (args[NextPostionalArgumentIndex].StartsWith("-")) { RaiseArgumentRequiredError(Arg.Argument); }
                            NewParsedValues.Add(Arg.Argument, args[NextPostionalArgumentIndex]);
                            NextPostionalArgumentIndex += 1;
                        }
                        catch (IndexOutOfRangeException) { RaiseArgumentRequiredError(Arg.Argument); }
                    }
                }
            }
        }

        internal void ParseOptionalArguments(string[] args, Dictionary<string, object> NewParsedValues)
        {
            ArgumentDataNamespace Arg;

            // Addition of default arguments values
            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);

                if (!Arg.Required)
                {
                    if (Arg.Type == "bool") { NewParsedValues.Add(Arg.Argument, Arg.Value == "true" ? true : false); }
                    else if (Arg.Type == "double") { NewParsedValues.Add(Arg.Argument, Convert.ToDouble(Arg.Value)); }
                    else { NewParsedValues.Add(Arg.Argument, Arg.Value); }
                }
            }

            // Addition of cli defined arguments values
            foreach (string ArgumentData in AddedArguments)
            {
                Arg = new ArgumentDataNamespace(ArgumentData);

                if (Array.Exists(args, element => element.StartsWith("--" + Arg.Argument)))
                {
                    SearchArgumentsValues(Arg, NewParsedValues, args);
                }

                if (Array.Exists(args, element => element.StartsWith("-" + Arg.Alias)) && Arg.Alias != "")
                {
                    SearchArgumentsValues(Arg, NewParsedValues, args, CheckAlias: true);
                }
            }
        }

        public dynamic ParseArgs(string[] args)
        {
            Dictionary<string, object> NewParsedValues = new Dictionary<string, object>();

            SortAddedArguments();
            DisplayHelp(args);
            CheckUnrecognizedArguments(args, NewParsedValues);
            ParsePostionalArguments(args, NewParsedValues);
            ParseOptionalArguments(args, NewParsedValues);

            IDictionary<string, object> DynamicDictObject = new System.Dynamic.ExpandoObject();
            foreach (var ParsedValue in NewParsedValues) { DynamicDictObject.Add(ParsedValue); }

            return (dynamic) DynamicDictObject;
        }
    }
 }
