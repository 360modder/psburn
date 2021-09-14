using System;
using System.IO;


namespace PsburnCliParser
{
    public class Utils
    {
        /// <summary>
        /// Creates a unique temporary directory
        /// </summary>
        /// <returns>Path of created temporary directory</returns>
        public static string CreateUniqueTempDirectory()
        {
            var UniqueTempDirPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            if (Directory.Exists(UniqueTempDirPath))
            {
                PrintColoredText("fatal: ", ConsoleColor.Red);
                Console.WriteLine("cannot assign a unique temporary directory, please re run this program.");
                Environment.Exit(1);
            }
            
            Directory.CreateDirectory(UniqueTempDirPath);
            return UniqueTempDirPath;
        }

        /// <summary>
        /// Prints colored text to console.
        /// <code>> Utils.PrintColoredText("caution", ConsoleColor.Yellow);</code>
        /// <code>caution</code>
        /// </summary>
        /// <param name="Text">Text to print</param>
        /// <param name="Color">Color of text</param>
        /// <param name="End">End of line</param>
        public static void PrintColoredText(string Text, ConsoleColor Color, string End = "")
        {
            Console.ForegroundColor = Color;
            Console.Write(Text + End);
            Console.ResetColor();
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
    }
}
