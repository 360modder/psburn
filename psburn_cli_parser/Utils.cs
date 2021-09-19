using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;


namespace PsburnCliParser
{
    public class Utils
    {
        /// <summary>
        /// Read lines from an embedded resource file and returns a string.
        /// </summary>
        /// <param name="EmbeddedFile">Path of embedded file</param>
        /// <returns>string</returns>
        public static string EmeddedFileReadAllText(string EmbeddedFile)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            StreamReader LoadedFileStream = new StreamReader(assembly.GetManifestResourceStream(EmbeddedFile));
            return LoadedFileStream.ReadToEnd();
        }

        /// <summary>
        /// Creates a unique temporary directory
        /// </summary>
        /// <returns>Path of created temporary directory</returns>
        public static string CreateUniqueTempDirectory()
        {
            var UniqueTempDirPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            if (Directory.Exists(UniqueTempDirPath))
            {
                Console.WriteLine("fatal: cannot assign a unique temporary directory, please re run this program.");
                Environment.Exit(1);
            }

            Directory.CreateDirectory(UniqueTempDirPath);
            return UniqueTempDirPath;
        }

        /// <summary>
        /// Unzip a embedded zipfile by making a local copy on system.
        /// </summary>
        /// <param name="EmbeddedZipPath">Path of embedded zipfile</param>
        /// <param name="ExtractDirectory">Extraction directory</param>
        /// <param name="TempPath">Extraction temporary directory</param>
        public static void UnzipEmbeddedZip(string EmbeddedZipPath, string ExtractDirectory, string TempPath)
        {
            string TempZipPath = Path.Combine(TempPath, "temp.zip");

            using (var Resource = Assembly.GetEntryAssembly().GetManifestResourceStream(EmbeddedZipPath))
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
                Console.WriteLine("fatal: failed to extract embedded zip.");
                Environment.Exit(1);
            }

            System.IO.File.Delete(TempZipPath);
        }

        /// <summary>
        /// Runs a subprocess from a executable with args.
        /// <code>> Utils.RunSubprocess("cmd", "/c=echo Hello World");</code>
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
}
