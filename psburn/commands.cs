using System;
using System.IO;


namespace Psburn
{
    public class PsburnCommand
    {
        public static void Create(string psscript, bool experimental, string executionpolicy, string scriptname,
                                  string scriptversion, bool blockcat, bool selfcontained, bool onedir,
                                  bool embedresources, bool noextract, string output, bool verbose)
        {
            // Reading powershell script and finding base name
            string[] PSScriptFileLines = File.ReadAllLines(psscript);
            string PsScriptBaseName = Path.GetFileNameWithoutExtension(psscript);
            string MainCsFile = $"{PsScriptBaseName}.cs";

            // cli parser utilities
            PowershellFeatures.PowershellParser ParsePowershellScript = new PowershellFeatures.PowershellParser(PSScriptFileLines, Verbosity: verbose);

            string ProgramDescription = ParsePowershellScript.ParseDescription();
            string ProgramUsage = ParsePowershellScript.ParseUsage();
            string[] ParsedExamples = ParsePowershellScript.ParseExamples();

            string[] ParsedParameters = ParsePowershellScript.ParseParameters();
            string[] ParsedDefaultCode = ParsePowershellScript.ParseDefaultCode(ParsedParameters);
            string[] ParsedHelp = ParsePowershellScript.ParseHelp();

            if (noextract)
            {
                // Handling powershell script comments
                PSScriptFileLines = PowershellFeatures.Passed.RemoveMultilineComments(PSScriptFileLines);
                PSScriptFileLines = PowershellFeatures.Passed.RemoveComments(PSScriptFileLines);

                // Experimental Features
                if (experimental)
                {
                    PSScriptFileLines = PowershellFeatures.Experimental.RemoveDoubleQuotes(PSScriptFileLines);
                    // PSScriptFileLines = PowershellFeatures.Experimental.RemoveDoubleQuotesDirectly(PSScriptFileLines);
                }

                // Checking for potential compile time errors
                PowershellFeatures.Passed.CheckErrors(PSScriptFileLines);
            }

            // Generate main script string from its lines
            string PSScriptFile = string.Join("", PSScriptFileLines);

            // Generate a psburn specific cs file from extract.cs or noextract.cs
            string[] CompileCsLines;

            if (noextract)
            {
                CompileCsLines = Utils.EmeddedFileReadAllLines("psburn.assets.noextract.cs");
            }

            else
            {
                CompileCsLines = Utils.EmeddedFileReadAllLines("psburn.assets.extract.cs");
            }

            foreach (string Line in CompileCsLines)
            {
                // Updating assembly info
                if (Line.Contains("[assembly: AssemblyTitle"))
                {
                    if (scriptname == "basename of input file") { scriptname = PsScriptBaseName; }
                    Utils.StringArrayReplace(CompileCsLines, Line, $"[assembly: AssemblyTitle(\"{scriptname}\")]");
                }

                else if (Line.Contains("[assembly: AssemblyProduct"))
                {
                    if (scriptname == "basename of input file") { scriptname = PsScriptBaseName; }
                    Utils.StringArrayReplace(CompileCsLines, Line, $"[assembly: AssemblyProduct(\"{scriptname}\")]");
                }

                else if (Line.Contains("[assembly: AssemblyFileVersion"))
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"[assembly: AssemblyFileVersion(\"{scriptversion}\")]");
                }

                // Add custom execution policy
                else if (Line.Contains("string ExPolicy"))
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring ExPolicy = \"{executionpolicy}\";");
                }

                // Dump powershell script in valid type of c# string in main c# file
                else if (Line.Contains("string PSScriptFile") && noextract)
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring PSScriptFile = @\"{PSScriptFile}\";");
                }

                // Block/Unblock script cat feature
                else if (Line.Contains("bool BlockCat") && blockcat)
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, "\t\t\tbool BlockCat = true;");
                }

                // Manage and set default values of parameters parsed from #@param
                else if (Line.Contains("string[] DefaultArgumentsCode"))
                {
                    string ParsedDefaultCodeString = Utils.ArrayStructureToString(ParsedDefaultCode);

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"DefaultArgumentsCode string array structure created e.i.\n{ParsedDefaultCodeString}\n");
                    }

                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring[] DefaultArgumentsCode = {ParsedDefaultCodeString};");
                }

                // Parse all parameters from #@param
                else if (Line.Contains("string[] ParsedParameters"))
                {
                    string ParsedParametersString = Utils.ArrayStructureToString(ParsedParameters);

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"ParsedParameters string array structure created e.i.\n{ParsedParametersString}\n");
                    }

                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring[] ParsedParameters = {ParsedParametersString};");
                }

                // Auto generate help texts from #@param
                else if (Line.Contains("string[] HelpArgumentsTexts"))
                {
                    string ParsedHelpString = Utils.ArrayStructureToString(ParsedHelp);

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"HelpArgumentsTexts string array structure created e.i.\n{ParsedHelpString}\n");
                    }

                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring[] HelpArgumentsTexts = {ParsedHelpString};");
                }

                // Add custom desciption to program
                else if (Line.Contains("string ProgramDescription"))
                {
                    if (ProgramDescription == "") { continue; }
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring ProgramDescription = \"{ProgramDescription}\";");
                }

                // Add custom examples to program
                else if (Line.Contains("string[] AllExamples"))
                {
                    string ParsedExamplesString = Utils.ArrayStructureToString(ParsedExamples);

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"AllExamples string array structure created e.i.\n{ParsedExamplesString}\n");
                    }

                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tstring[] AllExamples = {ParsedExamplesString};");
                }

                // Add custom usage to program
                else if (Line.Contains("ProgramUsage = \"\";"))
                {
                    if (ProgramUsage != "")
                    {
                        Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tProgramUsage = \"{ProgramUsage}\";");
                    }

                    else
                    {
                        Utils.StringArrayReplace(CompileCsLines, Line, "");
                    }
                }

                // Self-Contained app type
                else if (Line.Contains("bool OneDir = false;") && onedir)
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tbool OneDir = true;");
                }

                // Embeddable zips handles
                else if (Line.Contains("bool UnzipEmbeddedPowershellZip = false;") && selfcontained)
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tbool UnzipEmbeddedPowershellZip = true;");
                }

                else if (Line.Contains("bool UnzipEmbeddedResourceZip = false;") && embedresources)
                {
                    Utils.StringArrayReplace(CompileCsLines, Line, $"\t\t\tbool UnzipEmbeddedResourceZip = true;");
                }
            }

            // Save generated cs file in local drive
            if (output == "working directory") { File.WriteAllLines(MainCsFile, CompileCsLines); }
            else { File.WriteAllLines(output, CompileCsLines); }
        }

        public static void Cross(string psscript, string executionpolicy, bool blockcat, bool onedir, string output, bool verbose)
        {
            // Reading powershell script and finding base name
            string[] PSScriptFileLines = File.ReadAllLines(psscript);
            string PsScriptBaseName = Path.GetFileNameWithoutExtension(psscript);
            string MainPyFile = $"{PsScriptBaseName}.py";

            // cli parser utilities
            PowershellFeatures.PowershellParser ParsePowershellScript = new PowershellFeatures.PowershellParser(PSScriptFileLines, Verbosity: verbose);

            string ProgramDescription = ParsePowershellScript.ParseDescription();
            string ProgramUsage = ParsePowershellScript.ParseUsage();
            string[] ParsedExamples = ParsePowershellScript.ParseExamples();

            string[] ParsedParameters = ParsePowershellScript.ParseParameters(HelpText: true);
            string[] ParsedDefaultCode = ParsePowershellScript.ParseDefaultCode(ParsedParameters);

            // Generate a psburn specific py file from cross.py
            string[] CompilePyLines = Utils.EmeddedFileReadAllLines("psburn.assets.cross.py");

            foreach (string Line in CompilePyLines)
            {
                // Add custom execution policy
                if (Line.Contains("ExPolicy = \"Bypass\" # will come"))
                {
                    Utils.StringArrayReplace(CompilePyLines, Line, $"ExPolicy = \"{executionpolicy}\"");
                }

                // Add powershell script path
                else if (Line.Contains("PSScriptFile = \"path/to/psscript.ps1\" # will embed"))
                {
                    Utils.StringArrayReplace(CompilePyLines, Line, $"PSScriptFile = \"{PsScriptBaseName}.ps1\"");
                }

                // Block/Unblock script cat feature
                else if (Line.Contains("BlockCat = False # will come") && blockcat)
                {
                    Utils.StringArrayReplace(CompilePyLines, Line, "BlockCat = True");
                }

                // Manage and set default values of parameters parsed from #@param
                else if (Line.Contains("DefaultArgumentsCode = [] # will come"))
                {
                    string ParsedDefaultCodeString = Utils.ArrayStructureToString(ParsedDefaultCode, Brackets: "[,]");

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"DefaultArgumentsCode string array structure created e.i.\n{ParsedDefaultCodeString}\n");
                    }

                    Utils.StringArrayReplace(CompilePyLines, Line, $"DefaultArgumentsCode = {ParsedDefaultCodeString}");
                }

                // Parse all parameters from #@param
                else if (Line.Contains("ParsedParameters = [] # will come"))
                {
                    string ParsedParametersString = Utils.ArrayStructureToString(ParsedParameters, Brackets: "[,]");

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"ParsedParameters string array structure created e.i.\n{ParsedParametersString}\n");
                    }

                    Utils.StringArrayReplace(CompilePyLines, Line, $"ParsedParameters = {ParsedParametersString}");
                }


                // Add custom desciption to program
                else if (Line.Contains("ProgramDescription = \"dynamically auto compiled embedded powershell command script.\" # will come"))
                {
                    if (ProgramDescription == "") { continue; }
                    Utils.StringArrayReplace(CompilePyLines, Line, $"ProgramDescription = \"{ProgramDescription}\"");
                }

                // Add custom examples to program
                else if (Line.Contains("AllExamples = [] # will come"))
                {
                    string ParsedExamplesString = Utils.ArrayStructureToString(ParsedExamples, Brackets: "[,]");

                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine($"AllExamples string array structure created e.i.\n{ParsedExamplesString}\n");
                    }

                    Utils.StringArrayReplace(CompilePyLines, Line, $"AllExamples = {ParsedExamplesString}");
                }

                // Add custom usage to program
                else if (Line.Contains("ProgramUsage = \"\" # will come") && ProgramUsage != "")
                {
                    Utils.StringArrayReplace(CompilePyLines, Line, $"ProgramUsage = \"{ProgramUsage}\"");
                }

                // Self-Contained app type
                else if (Line.Contains("OneDir = False # will come") && onedir)
                {
                    Utils.StringArrayReplace(CompilePyLines, Line, $"OneDir = True");
                }
            }

            // Save generated py file in local drive
            if (output == "working directory") { File.WriteAllLines(MainPyFile, CompilePyLines); }
            else { File.WriteAllLines(output, CompilePyLines); }
        }

        public static void Build(string csfile, string psscript, bool debug, string powershellzip, string resourceszip,
                                 string cscpath, string icon, bool noconsole, bool uacadmin, bool onedir, string output,
                                 bool verbose)
        {
            // Try to auto detect csc.exe
            string cscexe = "csc.exe";

            if (cscpath == "auto detect")
            {
                try
                {
                    var cscpaths = Directory.GetDirectories("C:\\Windows\\Microsoft.NET\\Framework");
                    cscexe = Path.Join(cscpaths[cscpaths.Length - 1], "csc.exe");
                    cscexe = $"\"{cscexe}\"";
                }

                catch
                {
                    Utils.PrintColoredText("error: ", ConsoleColor.Red);
                    Console.WriteLine("failed to detect csc.exe path, try using --cscpath <cscpath>");
                    Environment.Exit(1);
                }
            }
            else { cscexe = $"\"{cscpath}\""; }

            // Creating distributable directory
            if (Directory.Exists("dist")) { Directory.Delete("dist", true); }
            Directory.CreateDirectory("dist/.cache");

            // Checking for script type e.i. extract.cs or noextract.cs
            string CsFileText = File.ReadAllText(csfile);
            bool noextract = true;
            if (CsFileText.Contains("ICSharpCode.SharpZipLib.Zip")) { noextract = false; }

            // Managing csc args supplied by user
            string ScriptEmbeddedPath = "Program.psscript.ps1";
            string PowershellZipEmbeddedPath = "Program.powershell.zip";
            string ResourcesZipEmbeddedPath = "Program.resources.zip";

            string cscargs = "/nologo /optimize ";

            if (noextract == false)
            {
                Utils.EmbeddedFileSave("psburn.assets.ICSharpCode.SharpZipLib.dll", "dist/ICSharpCode.SharpZipLib.dll");

                cscargs += $"/reference:\"dist/ICSharpCode.SharpZipLib.dll\" /resource:\"{Path.GetFullPath(psscript)}\",{ScriptEmbeddedPath} ";

                if (powershellzip != "no zip" && onedir)
                {
                    powershellzip = Path.GetFullPath(powershellzip);

                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"extracting {powershellzip}");

                    try { System.IO.Compression.ZipFile.ExtractToDirectory(powershellzip, "dist", true); }
                    catch
                    {
                        if (verbose)
                        {
                            Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                            Console.WriteLine($"{powershellzip} has some extraction errors.");
                        }
                    }
                }

                else if (powershellzip != "no zip")
                {
                    powershellzip = Path.GetFullPath(powershellzip);
                    cscargs += $"/resource:\"{powershellzip}\",{PowershellZipEmbeddedPath} ";
                }

                if (resourceszip != "no zip" && onedir)
                {
                    resourceszip = Path.GetFullPath(resourceszip);

                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"extracting {powershellzip}");

                    try { System.IO.Compression.ZipFile.ExtractToDirectory(resourceszip, "dist", true); }
                    catch
                    {
                        if (verbose)
                        {
                            Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                            Console.WriteLine($"{resourceszip} has some extraction errors.");
                        }
                    }
                }

                else if (resourceszip != "no zip")
                {
                    resourceszip = Path.GetFullPath(resourceszip);
                    cscargs += $"/resource:\"{resourceszip}\",{ResourcesZipEmbeddedPath} ";
                }
            }

            if (icon != "no icon") { cscargs += $"/win32icon:\"{Path.GetFullPath(icon)}\" "; }
            if (noconsole) { cscargs += "/target:winexe "; }
            if (uacadmin)
            {
                Utils.EmbeddedFileSave("psburn.assets.manifest.xml", "dist/.cache/manifest.xml");
                cscargs += $"/win32manifest:\"dist/.cache/manifest.xml\" ";
            }
            if (output != "working directory") { cscargs += $"/out:\"{output}\" "; }

            cscargs += $"\"{csfile}\" ";

            // Running windows inbuilt .NET Framework C# compiler to compile cs file
            Utils.PrintColoredText("info: ", ConsoleColor.Blue);
            Console.WriteLine("executing command.");
            Console.WriteLine($"{cscexe} {cscargs}\n");

            Utils.RunSubprocess(cscexe, cscargs);

            if (output != "working directory") { File.Move(output, $"dist/{Path.GetFileNameWithoutExtension(csfile)}.exe"); }
            else { File.Move($"{Path.GetFileNameWithoutExtension(csfile)}.exe", $"dist/{Path.GetFileNameWithoutExtension(csfile)}.exe"); }

            if (debug == false)
            {
                if (verbose)
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"cleaning up dist caches");
                }

                Directory.Delete("dist/.cache", true);
            }
        }

        public static void CBuild(string pyscript, string psscript, bool debug, string powershellzip, string resourceszip,
                                  bool onedir, bool noprompt, string output, bool verbose)
        {
            // Creating distributable directory
            if (Directory.Exists("dist")) { Directory.Delete("dist", true); }
            Directory.CreateDirectory("dist");

            // Copying python script to dist folder
            File.Copy(pyscript, Path.Combine("dist", Path.GetFileName(pyscript)));

            // Platform specific pyinstaller's --add-data separator
            string Separator = Utils.IsWindows ? ";" : ":";

            // Generating args for pyinstaller
            string PyInstallerArgs = $"{Path.GetFileName(pyscript)} --onefile ";

            PyInstallerArgs += $"--add-data \"{Path.GetFullPath(psscript)}{Separator}.\" ";

            if (powershellzip != "no zip")
            {
                Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                Console.WriteLine($"extracting {powershellzip}");

                try { System.IO.Compression.ZipFile.ExtractToDirectory(powershellzip, "dist", true); }
                catch
                {
                    if (verbose)
                    {
                        Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                        Console.WriteLine($"{powershellzip} has some extraction errors.");
                    }
                }
            }

            if (resourceszip != "no zip")
            {
                Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                Console.WriteLine($"extracting {powershellzip}");

                try { System.IO.Compression.ZipFile.ExtractToDirectory(resourceszip, "dist", true); }
                catch
                {
                    if (verbose)
                    {
                        Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                        Console.WriteLine($"{resourceszip} has some extraction errors.");
                    }
                }
            }

            if (onedir == false)
            {
                foreach (string Dir in Directory.GetDirectories("dist"))
                {
                    PyInstallerArgs += $"--add-data \"{Path.GetFullPath(Dir)}{Separator}{Path.GetRelativePath("dist", Dir)}\" ";
                }
            }

            if (noprompt == false)
            {
                Console.Write("supply any extra pyinstaller arguments: ");
                PyInstallerArgs += Console.ReadLine() + " ";
            }

            // Running pyinstaller to compile python script
            Utils.PrintColoredText("info: ", ConsoleColor.Blue);
            Console.WriteLine("executing command.");
            Console.WriteLine($"pyinstaller {PyInstallerArgs}\n");

            Directory.SetCurrentDirectory("dist");
            Utils.RunSubprocess("pyinstaller", PyInstallerArgs);
            Directory.SetCurrentDirectory("..");

            if (debug == false)
            {
                if (verbose)
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"cleaning up dist caches");
                }

                Directory.Move("dist/dist", "cdist");
                Directory.Delete("dist", true);
                Directory.Move("cdist", "dist");
            }

            if (onedir)
            {
                if (powershellzip != "no zip")
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"extracting {powershellzip}");

                    try { System.IO.Compression.ZipFile.ExtractToDirectory(powershellzip, "dist", true); }
                    catch
                    {
                        if (verbose)
                        {
                            Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                            Console.WriteLine($"{powershellzip} has some extraction errors.");
                        }
                    }
                }

                if (resourceszip != "no zip")
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine($"extracting {powershellzip}");

                    try { System.IO.Compression.ZipFile.ExtractToDirectory(resourceszip, "dist", true); }
                    catch
                    {
                        if (verbose)
                        {
                            Utils.PrintColoredText("caution: ", ConsoleColor.Red);
                            Console.WriteLine($"{resourceszip} has some extraction errors.");
                        }
                    }
                }
            }
        }
    }
}
