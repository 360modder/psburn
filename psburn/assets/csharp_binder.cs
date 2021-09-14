using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using PsburnCliParser;
using System.Reflection;
//[assembly: AssemblyTitle("psburn")] // File Description
//[assembly: AssemblyProduct("psburn")] // Product Name
//[assembly: AssemblyFileVersion("1.0.0")] // File Version


namespace csharp_binder
{
    class csharp_binder
    {
        /// <summary>
        /// Unzip a embedded zipfile by making a local copy on system.
        /// </summary>
        /// <param name="EmbeddedZipPath">Path of embedded zipfile</param>
        /// <param name="ExtractDirectory">Extraction directory</param>
        /// <param name="TempPath">Extraction temporary directory</param>
        public static void UnzipEmbeddedZip(string EmbeddedZipPath, string ExtractDirectory, string TempPath)
        {
            string TempZipPath = Path.Combine(TempPath, "temp.zip");

            using (var Resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedZipPath))
            {
                using (var File = new FileStream(TempZipPath, FileMode.Create, FileAccess.Write))
                {
                    Resource.CopyTo(File);
                }
            }

            try
            {
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(TempZipPath, ExtractDirectory, null);
            }

            catch
            {
                Utils.PrintColoredText("fatal: ", ConsoleColor.Red);
                Console.WriteLine("failed to extract embedded zip.");
                Environment.Exit(1);
            }
            
            System.IO.File.Delete(TempZipPath);
        }

        /// <summary>
        /// Read lines from an embedded resource file and returns a string.
        /// </summary>
        /// <param name="EmbeddedFile">Path of embedded file</param>
        /// <returns>string</returns>
        public static string EmeddedFileReadAllText(string EmbeddedFile)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader LoadedFileStream = new StreamReader(assembly.GetManifestResourceStream(EmbeddedFile));
            return LoadedFileStream.ReadToEnd();
        }

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
            string[] DefaultArgumentsCode = { };
            string[] HelpArgumentsTexts = { };
            string[] AllExamples = { };

            // Argument parser integration 
            string PSScriptFile = EmeddedFileReadAllText("csharp_binder.binding_psscript.ps1");
            string PSScriptName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            ArgumentParser PsburnArgumentParser = new ArgumentParser(PSScriptName, ProgramDescription, args, ParsedParameters);
            PsburnArgumentParser.DoHelp(HelpArgumentsTexts, AllExamples);
            PsburnArgumentParser.DoCat(PSScriptFile, BlockCat);
            string PSEmbedString = ArgumentParser.ParseDefaultArgs(DefaultArgumentsCode);
            PSEmbedString += PsburnArgumentParser.ParseArgs();

            // Unzip essentials to temporay directory
            string StorageDirectory = Utils.CreateUniqueTempDirectory();
            bool OneFile = OneDir ? false : true;

            if (UnzipEmbeddedResourceZip && OneFile)
            {
                UnzipEmbeddedZip("csharp_binder.resources.zip", StorageDirectory, StorageDirectory);
            }

            if (UnzipEmbeddedPowershellZip && OneFile)
            {
                UnzipEmbeddedZip("csharp_binder.powershell.zip", StorageDirectory, StorageDirectory);
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
            try { Directory.Delete(StorageDirectory, true); }
            catch { }
        }
    }
}
