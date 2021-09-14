using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Psburn
{
    /// <summary>
    /// Class for parsing json data coming from powershell script
    /// </summary>
    class PowershellParser
    {
        public string[] Code;
        public bool Verbose;

        /// <summary>
        /// PowershellParser Constructor
        /// </summary>
        /// <param name="PowershellScriptLines">String array of powershell script</param>
        /// <param name="Verbosity">Verbose logging</param>
        public PowershellParser(string[] PowershellScriptLines, bool Verbosity = false)
        {
            Code = PowershellScriptLines;
            Verbose = Verbosity;
        }

        /// <summary>
        /// Parses parameters from lines starting with #@param
        /// </summary>
        /// <param name="HelpText">Parse help text too</param>
        /// <returns>Parsed parameters data seprated by comma in a string array</returns>
        public string[] ParseParameters(bool HelpText = false)
        {
            dynamic ParsedJson;
            List<string> ParsedParameters = new List<string> { };
            int Count = 0;
            string Variable;
            string Value;
            string Type;
            string Required;
            string Alias;
            string Help;

            foreach (string Line in Code)
            {
                Count += 1;

                if (Line.StartsWith("#@param "))
                {
                    ParsedJson = JsonConvert.DeserializeObject(Line.Split("#@param ")[1].Replace("\n", ""));

                    Variable = $"{ParsedJson.variable}";
                    Value = $"{ParsedJson.value}";
                    Type = $"{ParsedJson.type}";
                    Required = $"{ParsedJson.required}";
                    Alias = $"{ParsedJson.alias}";

                    if (Variable == "")
                    {
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine($"Variable is undeclared in line {Count}, use");
                        Console.WriteLine("#@param {... \"variable\": \"<name>\"}");
                        Environment.Exit(1);
                    }

                    if (Type == "")
                    {
                        Type = "string";
                        if (Verbose)
                        {
                            Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                            Console.WriteLine($"variable {Variable} is set to type {Type}.");
                        }
                    }

                    if (Required == "")
                    {
                        Required = "false";
                        if (Verbose)
                        {
                            Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                            Console.WriteLine($"variable {Variable} dependency (required) set to {Required}.");
                        }
                    }

                    if (Alias == "" && Verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"variable {Variable} has no alias.");
                    }

                    if (Value == "" && Required == "false")
                    {
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine($"value should be defined for optional argument {Variable} or use.");
                        Console.WriteLine("#@param {... \"required\": \"true\"}");
                        Environment.Exit(1);
                    }

                    if (HelpText)
                    {
                        Help = $"{ParsedJson.help}";
                        if (Value == "" && Required == "true") { ParsedParameters.Add($"{Variable},{Type},{Required},{Value},{Alias},{Help}"); }
                        else { ParsedParameters.Add($"{Variable},{Type},{Required},{Value},{Alias},{Help} (default: {Value})"); }
                    }
                    else { ParsedParameters.Add($"{Variable},{Type},{Required},{Value},{Alias}"); }
                }
            }
            return ParsedParameters.ToArray();
        }

        /// <summary>
        /// Parses description from lines starting with #@parser
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Verbose"></param>
        /// <returns>Parsed program desciption</returns>
        public string ParseDescription()
        {
            dynamic ParsedJson;
            string Description = "";

            foreach (string Line in Code)
            {
                if (Line.StartsWith("#@parser "))
                {
                    ParsedJson = JsonConvert.DeserializeObject(Line.Split("#@parser ")[1].Replace("\n", ""));
                    Description = $"{ParsedJson.description}";

                    if (Description == "" && Verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine("parser description not set, program will have default description");
                    }
                }
            }
            return Description;
        }

        /// <summary>
        /// Parses usage from lines starting with #@parser
        /// </summary>
        /// <returns>Parsed Usage</returns>
        public string ParseUsage()
        {
            dynamic ParsedJson;
            string Usage = "";

            foreach (string Line in Code)
            {
                if (Line.StartsWith("#@parser "))
                {
                    ParsedJson = JsonConvert.DeserializeObject(Line.Split("#@parser ")[1].Replace("\n", ""));
                    Usage = $"{ParsedJson.usage}";

                    if (Usage == "" && Verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine("parser usage not set, program will auto generate usage.");
                    }
                }
            }
            return Usage;
        }

        /// <summary>
        /// Parses examples from #@parser
        /// </summary>
        /// <returns>Parsed examples array</returns>
        public string[] ParseExamples()
        {
            dynamic ParsedJson;
            List<string> Examples = new List<string> { };

            string ExamplesString;

            foreach (string Line in Code)
            {
                if (Line.StartsWith("#@parser "))
                {
                    ParsedJson = JsonConvert.DeserializeObject(Line.Split("#@parser ")[1].Replace("\n", ""));

                    ExamplesString = $"{ParsedJson.examples}";

                    if (ExamplesString == "")
                    {
                        if (Verbose)
                        {
                            Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                            Console.WriteLine("parser examples not set, program will have default examples.");
                        }
                    }

                    else
                    {
                        foreach (string Ex in ParsedJson.examples) { Examples.Add(Ex); }
                    }
                }
            }
            return Examples.ToArray();
        }

        /// <summary>
        /// Parses and generates help text from lines starting with #@param
        /// </summary>
        /// <returns>Postional and optional help texts inside a string array</returns>
        public string[] ParseHelp()
        {
            dynamic ParsedJson;
            List<string> FormattedPostionalHelp = new List<string> { };
            List<string> FormattedOptionalHelp = new List<string> { };
            int PostionalCount = 0;
            int OptionalCount = 0;

            string Variable;
            string Alias;
            string Value;
            string Type;
            string Required;
            string Help;

            string OutLine;
            string MoveToNewline;

            foreach (string Line in Code)
            {
                if (Line.StartsWith("#@param "))
                {
                    ParsedJson = JsonConvert.DeserializeObject(Line.Split("#@param ")[1].Replace("\n", ""));

                    Variable = $"{ParsedJson.variable}";
                    Alias = $"{ParsedJson.alias}";
                    Value = $"{ParsedJson.value}";
                    Type = $"{ParsedJson.type}";
                    Required = $"{ParsedJson.required}";
                    Help = $"{ParsedJson.help}";

                    if (Help == "" && Verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"help text not set for variable {ParsedJson.variable}, program will have empty help text.");
                    }

                    if (Required == "true" && PostionalCount == 0)
                    {
                        FormattedPostionalHelp.Add("postional arguments:");
                        PostionalCount += 1;
                    }

                    else if (Required == "false" && OptionalCount == 0)
                    {
                        FormattedOptionalHelp.Add("optional arguments:");
                        OptionalCount += 1;
                    }

                    OutLine = "  ";
                    if (Alias != "") { OutLine += $"-{Alias}, "; }
                    OutLine += $"--{Variable} ";
                    if (Type != "bool") { OutLine += $"<{Variable.ToUpper()}>"; }

                    if (30 <= OutLine.Length)
                    {
                        if (Help == "") { MoveToNewline = ""; }
                        else { MoveToNewline = @"\n"; }

                        OutLine = OutLine + MoveToNewline + string.Format("{0,-30} {1}", "", Help);
                    }

                    else
                    {
                        OutLine = string.Format("{0,-30} {1}", OutLine, Help);
                    }

                    if (Required == "true") { FormattedPostionalHelp.Add(OutLine); }
                    else if (Required == "false") { FormattedOptionalHelp.Add(OutLine + $" (default: {Value})"); }
                }
            }

            if (FormattedPostionalHelp.Count != 0) { FormattedPostionalHelp.Add(""); }
            FormattedOptionalHelp.ForEach(item => FormattedPostionalHelp.Add(item));

            return FormattedPostionalHelp.ToArray();
        }

        /// <summary>
        /// Generates a powershell code which has variable and values set from parsed code
        /// </summary>
        /// <param name="ParsedParameters">Parameters parsed from method ParseParameters</param>
        /// <returns>Default variables powershell code</returns>
        public string[] ParseDefaultCode(string[] ParsedParameters)
        {
            string Outline = "";
            List<string> LinesToAppend = new List<string> { };

            string[] DataArray;
            string Variable;
            string Value;
            string Type;
            string Required;


            foreach (string Line in ParsedParameters)
            {
                DataArray = Line.Split(",");

                Variable = DataArray[0];
                Value = DataArray[3];
                Type = DataArray[1];
                Required = DataArray[2];

                if (Required == "false")
                {
                    if (Type == "string") { Outline = $"${Variable} = '{Value}'"; }
                    else if (Type == "bool") { Outline = $"${Variable} = ${Value}"; }
                    else if (Type == "float") { Outline = $"${Variable} = [double] '{Value}'.Replace('@', '')"; }

                    LinesToAppend.Add(Outline);
                }
            }
            return LinesToAppend.ToArray();
        }
    }
}
