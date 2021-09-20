using System;
using System.IO;
using System.Collections.Generic;


namespace Psburn
{
    public class PsburnCommands
    {
        public static void Create(string psscript, string output, bool py, bool selfcontained, bool embedresources,
                                  bool onedir, string executionpolicy, bool blockcat, bool verbose)
        {
            PowershellScriptParser ParsePowershellScript = new PowershellScriptParser(File.ReadAllLines(psscript), Verbosity: verbose);
            string ProgramDescription = ParsePowershellScript.ParseDescription();
            string[] ParsedParameters = ParsePowershellScript.ParseParameters();

            if (!py)
            {
                string[] CsCodeLines = Utils.EmeddedFileReadAllLines("psburn.assets.csharp_binder.cs");

                foreach (string Line in CsCodeLines)
                {
                    if (Line.Contains("string ProgramDescription"))
                    {
                        if (ProgramDescription == "") { continue; }
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tstring ProgramDescription = \"{ProgramDescription}\";");
                    }

                    else if (Line.Contains("string ExPolicy"))
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tstring ExPolicy = \"{executionpolicy}\";");
                    }

                    else if (Line.Contains("bool BlockCat") && blockcat)
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, "\t\t\tbool BlockCat = true;");
                    }

                    else if (Line.Contains("bool OneDir = false;") && onedir)
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tbool OneDir = true;");
                    }

                    else if (Line.Contains("bool UnzipEmbeddedResourceZip = false;") && embedresources)
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tbool UnzipEmbeddedResourceZip = true;");
                    }

                    else if (Line.Contains("bool UnzipEmbeddedPowershellZip = false;") && selfcontained)
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tbool UnzipEmbeddedPowershellZip = true;");
                    }

                    else if (Line.Contains("string[] ParsedParameters"))
                    {
                        Utils.StringArrayReplace(CsCodeLines, Line, $"\t\t\tstring[] ParsedParameters = {Utils.ArrayStructureToString(ParsedParameters)};");
                    }
                }

                if (output == "working directory")
                {
                    File.WriteAllLines($"{Path.GetFileNameWithoutExtension(psscript)}.cs", CsCodeLines);
                }
                else { File.WriteAllLines(output, CsCodeLines); }
            }

            else
            {
                string[] PythonCodeLines = Utils.EmeddedFileReadAllLines("psburn.assets.python_binder.py");

                foreach (string Line in PythonCodeLines)
                {
                    if (Line.Contains("ProgramDescription = \"dynamic embedded powershell script\" # will come"))
                    {
                        if (ProgramDescription == "") { continue; }
                        Utils.StringArrayReplace(PythonCodeLines, Line, $"ProgramDescription = \"{ProgramDescription}\"");
                    }

                    else if (Line.Contains("ExPolicy = \"Bypass\" # will come"))
                    {
                        Utils.StringArrayReplace(PythonCodeLines, Line, $"ExPolicy = \"{executionpolicy}\"");
                    }

                    else if (Line.Contains("BlockCat = False # will come") && blockcat)
                    {
                        Utils.StringArrayReplace(PythonCodeLines, Line, "BlockCat = True");
                    }

                    else if (Line.Contains("OneDir = False # will come") && onedir)
                    {
                        Utils.StringArrayReplace(PythonCodeLines, Line, $"OneDir = True");
                    }

                    else if (Line.Contains("ParsedParameters = [] # will come"))
                    {
                        Utils.StringArrayReplace(PythonCodeLines, Line, $"ParsedParameters = {Utils.ArrayStructureToString(ParsedParameters, Brackets: "[,]")}");
                    }

                    else if (Line.Contains("PSScriptFile = \"binding_psscript.ps1\" # will come"))
                    {
                        Utils.StringArrayReplace(PythonCodeLines, Line, $"PSScriptFile = \"{Path.GetFileNameWithoutExtension(psscript)}.ps1\"");
                    }
                }

                if (output == "working directory")
                {
                    File.WriteAllLines($"{Path.GetFileNameWithoutExtension(psscript)}.py", PythonCodeLines);
                }
                else { File.WriteAllLines(output, PythonCodeLines); }
            }
        }

        public static void Build(string psscript, string binderfile, string powershellzip, string resourceszip,
                                 bool onedir, string icon, bool noconsole, bool uacadmin, string cscpath,
                                 bool merge, bool pyinstallerprompt, bool debug, bool verbose)
        {
            if (Directory.Exists("dist")) { Directory.Delete("dist", true); }
            Directory.CreateDirectory("dist/.cache");

            if (binderfile.EndsWith(".cs"))
            {
                string CscExe = (cscpath == "auto detect") ? Utils.DetectCscPath() : $"\"{cscpath}\"";
                Utils.EmbeddedFileSave("psburn.assets.ICSharpCode.SharpZipLib.dll", "dist/ICSharpCode.SharpZipLib.dll");
                Utils.EmbeddedFileSave("psburn.assets.PsburnCliParser.dll", "dist/PsburnCliParser.dll");

                List<string> CscArgs = new List<string> {
                "/nologo", "/optimize",
                "/reference:dist/PsburnCliParser.dll",
                //"/reference:dist/ICSharpCode.SharpZipLib.dll",
                $"/resource:\"{psscript}\",csharp_binder.binding_psscript.ps1" };

                if (powershellzip != "no zip" && onedir) { Utils.ExtractZip(powershellzip); }
                else if (powershellzip != "no zip") { CscArgs.Add($"/resource:\"{powershellzip}\",csharp_binder.powershell.zip"); }
                if (resourceszip != "no zip" && onedir) { Utils.ExtractZip(resourceszip); }
                else if (resourceszip != "no zip") { CscArgs.Add($"/resource:\"{resourceszip}\",csharp_binder.resources.zip"); }

                if (icon != "no icon") { CscArgs.Add($"/win32icon:\"{icon}\""); }
                if (noconsole) { CscArgs.Add("/target:winexe"); }
                if (uacadmin)
                {
                    Utils.EmbeddedFileSave("psburn.assets.manifest.xml", "dist/.cache/manifest.xml");
                    CscArgs.Add($"/win32manifest:dist/.cache/manifest.xml");
                }

                CscArgs.Add($"/out:dist/{Path.GetFileNameWithoutExtension(binderfile)}.exe");
                CscArgs.Add($"\"{binderfile}\"");

                Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                Console.WriteLine("executing command.");
                Console.WriteLine($"{CscExe} {string.Join(" ", CscArgs)}\n");

                Utils.RunSubprocess(CscExe, string.Join(" ", CscArgs));

                if (File.Exists($"dist/{Path.GetFileNameWithoutExtension(binderfile)}.exe") && merge)
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine("merging dll into exe.");

                    Utils.RunSubprocess(
                        Path.Join(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "ILMerge/ILMerge.exe"),
                        $"/out:dist/merged.exe dist/{Path.GetFileNameWithoutExtension(binderfile)}.exe dist/PsburnCliParser.dll dist/ICSharpCode.SharpZipLib.dll"
                    );

                    File.Delete("dist/merged.pdb");
                    File.Delete($"dist/{Path.GetFileNameWithoutExtension(binderfile)}.exe");
                    File.Delete("dist/PsburnCliParser.dll");
                    File.Delete("dist/ICSharpCode.SharpZipLib.dll");
                    File.Move("dist/merged.exe", $"dist/{Path.GetFileNameWithoutExtension(binderfile)}.exe");
                }

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

            else if (binderfile.EndsWith(".py"))
            {
                File.Copy(binderfile, Path.Combine("dist", Path.GetFileName(binderfile)));
                string Separator = Utils.IsWindows ? ";" : ":";

                List<string> PyInstallerArgs = new List<string> { $"\"{Path.GetFileName(binderfile)}\"", "--onefile"};
                PyInstallerArgs.Add($"--add-data \"{Path.GetFullPath(psscript)}{Separator}.\"");

                if (!onedir)
                {
                    if (powershellzip != "no zip") { Utils.ExtractZip(powershellzip); }
                    if (resourceszip != "no zip") { Utils.ExtractZip(resourceszip); }

                    foreach (string Dir in Directory.GetDirectories("dist"))
                    {
                        PyInstallerArgs.Add($"--add-data \"{Path.GetFullPath(Dir)}{Separator}{Path.GetRelativePath("dist", Dir)}\"");
                    }
                }

                if (icon != "no icon") { PyInstallerArgs.Add($"--icon \"{icon}\""); }
                if (noconsole) { PyInstallerArgs.Add("--noconsole"); }
                if (uacadmin) { PyInstallerArgs.Add("--uac-admin"); }

                if (pyinstallerprompt)
                {
                    Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                    Console.WriteLine("current command.");
                    Console.WriteLine($"pyinstaller {string.Join(" ", PyInstallerArgs)}\n");
                    Console.Write("supply any extra pyinstaller arguments: ");
                    PyInstallerArgs.Add(Console.ReadLine());
                }

                Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                Console.WriteLine("executing command.");
                Console.WriteLine($"pyinstaller {string.Join(" ", PyInstallerArgs)}\n");

                Directory.SetCurrentDirectory("dist");
                Utils.RunSubprocess("pyinstaller", string.Join(" ", PyInstallerArgs));
                Directory.SetCurrentDirectory("..");

                if (debug == false)
                {
                    if (verbose)
                    {
                        Utils.PrintColoredText("info: ", ConsoleColor.Blue);
                        Console.WriteLine("cleaning up dist caches");
                    }

                    Directory.Move("dist/dist", "cdist");
                    Directory.Delete("dist", true);
                    Directory.Move("cdist", "dist");
                }

                if (onedir)
                {
                    if (powershellzip != "no zip") { Utils.ExtractZip(powershellzip); }
                    if (resourceszip != "no zip") { Utils.ExtractZip(resourceszip); }
                }
            }

            else
            {
                Utils.PrintColoredText("error: ", ConsoleColor.Red);
                Console.WriteLine("supplied binder file is neither c# nor python file.");
                Environment.Exit(1);
            }
        }
    }
}
