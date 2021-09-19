using System;
using System.IO;
using PsburnCliParser;
using System.Collections.Generic;
using System.Reflection;
// Change some assembly information if required
//[assembly: AssemblyTitle("psburn")] // File Description
//[assembly: AssemblyProduct("psburn")] // Product Name
//[assembly: AssemblyFileVersion("1.0.0")] // File Version


namespace csharp_binder
{
    class csharp_binder
    {
        public static void Main(string[] args)
        {
            // will come - means that these variables will be pushed here at time of creation
            string ProgramDescription = "dynamic embedded powershell script";
            string ExPolicy = "Bypass";
            bool BlockCat = false;
            bool OneDir = false;
            bool UnzipEmbeddedResourceZip = false;
            bool UnzipEmbeddedPowershellZip = false;
            string[] ParsedParameters = { };

            // Some extra variables
            string PSScriptFile = Utils.EmeddedFileReadAllText("csharp_binder.binding_psscript.ps1");
            string PSScriptName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

            // Argument parser integration
            ArgumentParser Parser = new ArgumentParser(Description: ProgramDescription);
            Parser.AddArgumentFromParsedParameters(ParsedParameters);
            Parser.AddArgument("--cat", Default: "false", Type: "bool", Help: "instead of running cat powershell script into console");
            IDictionary<string, object> ParsedArgs = Parser.ParseArgs(args);

            // Displaying powershell script if --cat argument is supplied
            if ((bool) ParsedArgs["cat"])
            {
                if (BlockCat)
                {
                    Console.WriteLine("error: cat is blocked during runtime");
                    Environment.Exit(1);
                }

                else
                {
                    Console.WriteLine(PSScriptFile);
                    Environment.Exit(0);
                }
            }

            // Code generation for supplied arguments
            ArgumentDataNamespace Arg;
            string PSEmbedString = "";

            foreach (string ParsedArgumentData in ParsedParameters)
            {
                Arg = new ArgumentDataNamespace(ParsedArgumentData);
                if (Arg.Type == "bool") { PSEmbedString += string.Format("${0} = ${1}\n", Arg.Argument, ParsedArgs[Arg.Argument].ToString().ToLower()); }
                else if (Arg.Type == "double") { PSEmbedString += string.Format("${0} = {1}\n", Arg.Argument, ParsedArgs[Arg.Argument]); }
                else if (Arg.Type == "string") { PSEmbedString += string.Format("${0} = '{1}'\n", Arg.Argument, ParsedArgs[Arg.Argument]); }
            }

            // Unzip essentials to temporay directory
            string StorageDirectory = Utils.CreateUniqueTempDirectory();
            bool OneFile = OneDir ? false : true;

            if (UnzipEmbeddedResourceZip && OneFile)
            {
                Utils.UnzipEmbeddedZip("csharp_binder.resources.zip", StorageDirectory, StorageDirectory);
            }

            if (UnzipEmbeddedPowershellZip && OneFile)
            {
                Utils.UnzipEmbeddedZip("csharp_binder.powershell.zip", StorageDirectory, StorageDirectory);
            }

            // Determining the path of powershell executable
            string PSScriptRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string Executable = Utils.IsWindows ? "pwsh.exe" : "pwsh";
            Executable = UnzipEmbeddedPowershellZip && OneFile ? Path.Combine(StorageDirectory, "pwsh", Executable) : Path.Combine(PSScriptRoot, "pwsh", Executable);
            if (!File.Exists(Executable))
            {
                Executable = Utils.IsWindows ? "powershell.exe" : "pwsh";
            }

            // Writting a new powershell script to temporary path
            PSEmbedString = string.Format("$Executable = '{0}'\n", Executable) + PSEmbedString;
            PSEmbedString = string.Format("$PSScriptTempRoot = '{0}'\n", StorageDirectory) + PSEmbedString;
            PSEmbedString = string.Format("$PSScriptRoot = '{0}'\n", PSScriptRoot) + PSEmbedString;
            PSEmbedString += "\n" + PSScriptFile;

            string TempScriptPath = Path.Combine(StorageDirectory, PSScriptName + ".ps1");
            File.WriteAllText(TempScriptPath, PSEmbedString);

            // Final call to powershell
            Utils.RunSubprocess(Executable, string.Format("-ExecutionPolicy {0} -File \"{1}\"", ExPolicy, TempScriptPath));

            // Perform clean actions
            try { Directory.Delete(StorageDirectory, true); } catch { }
        }
    }
}
