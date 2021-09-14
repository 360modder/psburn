using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


namespace Psburn
{
    /// <summary>
    /// Utilities class for psburn
    /// </summary>
    class Utils
    {
		/// <summary>
		/// Save an embedded file in local drive
		/// </summary>
		/// <param name="EmbeddedFile">Path of embedded file</param>
		/// <param name="PathOfFile">Path where to save file</param>
		public static void EmbeddedFileSave(string EmbeddedFile, string PathOfFile)
        {
			using (var Resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedFile))
			{
				using (var File = new FileStream(PathOfFile, FileMode.Create, FileAccess.Write))
				{
					Resource.CopyTo(File);
				}
			}
		}
		/// <summary>
		/// Read all lines from an embbeded resource file and returns a string array.
		/// <code>> Psburn.Utils.EmmededFileReadAllLines("psburn.assets.psboilerplate.cs");</code>
		/// <code>string.String[]</code>
		/// </summary>
		/// <param name="EmbeddedFile">Path of embedded file</param>
		/// <returns>string.String[]</returns>
		public static string[] EmeddedFileReadAllLines(string EmbeddedFile)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			StreamReader LoadedFileStream = new StreamReader(assembly.GetManifestResourceStream(EmbeddedFile));

			List<string> FileLines = new List<string>();
			string Line;


			while ((Line = LoadedFileStream.ReadLine()) != null)
			{
				FileLines.Add(Line);
			}

			LoadedFileStream.Close();
			return FileLines.ToArray();
		}

		/// <summary>
		/// Runs a subprocess from a executable with args.
		/// <code>> Psburn.Utils.RunSubprocess("cmd", "/c=echo Hello World");</code>
		/// <code>Hello World</code>
		/// </summary>
		/// <param name="file">Executable path</param>
		/// <param name="args">Args to supply</param>
		/// <returns>subprocess exit code</returns>
		public static int RunSubprocess(string file, string args, bool shell=false)
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
		/// Replace a string in string array.
		/// <code>> Psburn.Utils.StringArrayReplace(new string[] {"Hello", "!"}, "Hello", "World");</code>
		/// <code>{"World", "!"}</code>
		/// </summary>
		/// <param name="Code">String array</param>
		/// <param name="Line">String to replace</param>
		/// <param name="ReplaceLine">Replacing string</param>
		public static void StringArrayReplace(string[] Code, string Line, string ReplaceLine)
        {
			Code[Array.IndexOf(Code, Line)] = ReplaceLine;
		}

		/// <summary>
		/// Prints colored text to console.
		/// <code>> Psburn.Utils.PrintColoredText("caution", ConsoleColor.Yellow);</code>
		/// <code>caution</code>
		/// </summary>
		/// <param name="Text">Text to print</param>
		/// <param name="Color">Color of text</param>
		/// <param name="End">End of line</param>
		public static void PrintColoredText(string Text, ConsoleColor Color, string End="")
        {
			Console.ForegroundColor = Color;
			Console.Write(Text + End);
			Console.ResetColor();
        }

		/// <summary>
		/// Constructs string arrays to strings arrays like structure inside a string
		/// <code>> Psburn.Utils.ArrayStructureToString(new string[] {"1", "2", "3"});</code>
		/// <code>{"1", "2", "3", }</code>
		/// </summary>
		/// <param name="Array">String array to use</param>
		/// /// <param name="Brackets">Opening closing brackets to use</param>
		/// <returns>String which has same structure as array</returns>
		public static string ArrayStructureToString(string[] Array, string Brackets = "{,}")
        {
			string OutLine = "";

			foreach (string Line in Array)
			{
				OutLine += $"\"{Line}\", ";
			}

			OutLine = Brackets.Split(",")[0] + OutLine + Brackets.Split(",")[1];

			return OutLine;
		}

		/// <summary>
		/// Try to detect the path of highest version of csc.exe
		/// </summary>
		/// <returns>Path of csc.exe</returns>
		public static string DetectCscPath()
		{
			try
			{
				string[] CscPaths = Directory.GetDirectories("C:\\Windows\\Microsoft.NET\\Framework");
				return $"\"{Path.Join(CscPaths[CscPaths.Length - 1], "csc.exe")}\"";
			}

			catch
			{
				PrintColoredText("error: ", ConsoleColor.Red);
				Console.WriteLine("failed to detect csc.exe path, try using --cscpath <cscpath>");
				Environment.Exit(1);
				return "";
			}
		}

		/// <summary>
		/// Extract zipfile to a directory with some messages 
		/// </summary>
		/// <param name="Zipfile">Path of zip file</param>
		/// <param name="Dst">Path of destination directory</param>
		public static void ExtractZip(string Zipfile, string Dst = "dist")
		{
			PrintColoredText("info: ", ConsoleColor.Blue);
			Console.WriteLine($"extracting {Zipfile}");

			try { System.IO.Compression.ZipFile.ExtractToDirectory(Zipfile, Dst, true); }
			catch
			{
				PrintColoredText("caution: ", ConsoleColor.Red);
				Console.WriteLine($"{Zipfile} has some extraction errors.");

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
