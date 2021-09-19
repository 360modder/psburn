using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Psburn
{
    /// <summary>
    /// Class for parsing json data coming from powershell script
    /// </summary>
    class PowershellScriptParser
    {
        public string[] Code;
        public bool Verbose;

        /// <summary>
        /// PowershellScriptParser Constructor
        /// </summary>
        /// <param name="PowershellScriptLines">String array of powershell script</param>
        /// <param name="Verbosity">Verbose logging</param>
        public PowershellScriptParser(string[] PowershellScriptLines, bool Verbosity = false)
        {
            Code = PowershellScriptLines;
            Verbose = Verbosity;
        }

        /// <summary>
        /// Parses description from lines starting with #@parser
        /// </summary>
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
                        Console.WriteLine("parser description not set, program will have default description.");
                    }
                }
            }
            return Description;
        }

        /// <summary>
        /// Parses parameters from lines starting with #@param
        /// </summary>
        /// <returns>Parsed parameters data seprated by comma in a string array</returns>
        public string[] ParseParameters()
        {
            List<string> ParsedParameters = new List<string> { };
            dynamic ParsedJson;
            string Variable;
            string Value;
            string Type;
            string Required;
            string Alias;
            string Help;
            int Count = 0;

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
                    Help = $"{ParsedJson.help}";

                    if (Variable == "")
                    {
                        Utils.PrintColoredText("error: ", ConsoleColor.Red);
                        Console.WriteLine($"variable is undeclared in line {Count}, use");
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
                    
                    ParsedParameters.Add($"{Variable},{Type},{Required},{Value},{Alias},{Help}");
                }
            }
            return ParsedParameters.ToArray();
        }
    }
}
