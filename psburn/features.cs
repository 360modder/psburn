using System;
using System.Text.RegularExpressions;
using Psburn;


namespace PowershellFeatures
{
    /// <summary>
    /// Class for all experimental powershell features for psburn
    /// </summary>
    class Experimental
    {
        /// <summary>
        /// Remove double quotes from a powershell type string array with format specifier.
		/// <code>> PowershellFeatures.Experimental.RemoveDoubleQuotes(new string[] {"$var = 1", "Write-Output \"value of var is $var\""});</code>
		/// <code>{"$var = 1", "'value of var is {0}' -f $var"}</code>
        /// </summary>
        /// <param name="Code">String array of powershell script</param>
        /// <returns>string.String[]</returns>
        public static string[] RemoveDoubleQuotes(string[] Code)
        {
            int InlineVariables;
            string UpdatedLine;
            string MatchedString;
            int MatchCount;
            int Count;
            int LineCount = 0;

            foreach (string Line in Code)
            {
                InlineVariables = Regex.Matches(Line, @"\$[a-zA-Z0-9_:]*( =|=)").Count;
                MatchCount = Regex.Matches(Line, @"\$[a-zA-Z0-9_:\.]*").Count;
                LineCount += 1;

                if (InlineVariables == 0) // checks wheter given line is variable init state or not
                {
                    if (Line.Contains("\"") == false) { continue; } // if line dosen't contains double qoutes then continue to next iteration
                    UpdatedLine = Line.Replace("\"", "'");
                    UpdatedLine = UpdatedLine.Trim('\n');

                    Count = 0;
                    foreach (var Match in Regex.Matches(Line, @"\$[a-zA-Z0-9_:\.]*")) // iterate through matched variables in line
                    {
                        MatchedString = Match.ToString();
                        UpdatedLine = UpdatedLine.Replace(MatchedString, "{" + $"{Count}" + "}"); // powershell specific formatting

                        if (Count == 0) { UpdatedLine += " -f "; } // format specifier

                        // joining variables
                        if (MatchCount == Count + 1) { UpdatedLine += MatchedString; }
                        else { UpdatedLine += MatchedString + ", "; }

                        Count += 1;
                    }

                    // fixing outputs with Write-Output
                    if (Line.Contains("Write-Output")) { UpdatedLine = UpdatedLine.Replace("Write-Output", "").Trim(); }

                    UpdatedLine += "\n";

                    Utils.StringArrayReplace(Code, Line, UpdatedLine);

                    Utils.PrintColoredText("caution: ", ConsoleColor.Yellow);
                    Console.WriteLine($"line {LineCount} changed to\n{UpdatedLine}");
                }
            }
            return Code;
        }

        /// <summary>
        /// Remove double quotes from a powershell type string array without format specifier.
        /// This function would always remain a experimental function.
        /// <code>> PowershellFeatures.Experimental.RemoveDoubleQuotesDirectly(new string[] {"$var = 1", "Write-Output \"value of var is $var\""});</code>
        /// <code>{"$var = 1", "Write-Output 'value of var is $var'"}</code>
        /// </summary>
        /// <param name="Code">String array of powershell script</param>
        /// <returns>string.String[]</returns>
        public static string[] RemoveDoubleQuotesDirectly(string[] Code)
        {
            int LineCount = 0;

            foreach (string Line in Code)
            {
                LineCount += 1;

                if (Line.Contains("\""))
                {
                    Utils.StringArrayReplace(Code, Line, Line.Replace("\"", "'"));

                    Utils.PrintColoredText("caution: ", ConsoleColor.Yellow);
                    Console.WriteLine($"line {LineCount} changed to\n{Line.Replace("\"", "'")}\n");
                }
            }

            return Code;
        }
    }

    /// <summary>
    /// Class for all passed powershell features for psburn
    /// </summary>
    class Passed
    {
        /// <summary>
        /// Checks for potential errors which can cause compile time errors
        /// </summary>
        /// <param name="Code"></param>
        public static void CheckErrors(string[] Code)
        {
            int LineCount = 0;
            int ErrorIndex;

            foreach (string Line in Code)
            {
                LineCount += 1;

                if (Line.Contains("\""))
                {
                    ErrorIndex = Line.IndexOf('"');
                    Console.Write(Line);

                    for (int i = 0; i < ErrorIndex; i++) { Console.Write(" "); }

                    Utils.PrintColoredText("^", ConsoleColor.Red, End: "\n");
                    Utils.PrintColoredText("error: ", ConsoleColor.Red);
                    Console.WriteLine($"double qoutes found in line {LineCount} at index {ErrorIndex}.");
                    Console.WriteLine("compilation terminated.");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Remove comments starting with # from string array.
        /// </summary>
        /// <param name="Code">String array of powershell script</param>
        /// <returns></returns>
        public static string[] RemoveComments(string[] Code)
        {
            foreach (string Line in Code)
            {
                if (Line.StartsWith("#")) { Utils.StringArrayReplace(Code, Line, ""); }
                else if (Line.EndsWith("\n")) { }
                else { Utils.StringArrayReplace(Code, Line, Line + "\n"); }
            }
            return Code;
        }

        /// <summary>
        /// Remove comments opening and closing with <# and #> in string array.
        /// </summary>
        /// <param name="Code">String array of powershell script</param>
        /// <returns></returns>
        public static string[] RemoveMultilineComments(string[] Code)
        {
            bool ContinueTill = false;

            foreach (string Line in Code)
            {
                if (Line.Contains("<#") && Line.Contains("#>")) { Code[Array.IndexOf(Code, Line)] = ""; }

                else if (Line.Contains("<#"))
                {
                    Utils.StringArrayReplace(Code, Line, "");
                    ContinueTill = true;
                }

                else if (ContinueTill)
                {
                    Utils.StringArrayReplace(Code, Line, "");
                    if (Line.Contains("#>")) { ContinueTill = false; }
                }

                else if (Line.EndsWith("\n")) { }

                else { Utils.StringArrayReplace(Code, Line, Line + "\n"); }
            }
            return Code;
        }
    }
}
