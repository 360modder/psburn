using System;
using System.CommandLine;
using System.CommandLine.Invocation;


namespace psburn
{
	internal class psburn
	{
		public static string[] EmmededFileReadAllLines(string EmbeddedFile)
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			System.IO.StreamReader LoadedFileStream = new System.IO.StreamReader(assembly.GetManifestResourceStream(EmbeddedFile));

			System.Collections.Generic.List<string> FileLines = new System.Collections.Generic.List<string>();
			string line;


			while ((line = LoadedFileStream.ReadLine()) != null)
			{
				FileLines.Add(line);
			}

			LoadedFileStream.Close();
			return FileLines.ToArray();
		}

		public static int RunSubprocess(string file, string args)
		{
			try
			{
				var process = new System.Diagnostics.Process
				{
					StartInfo = new System.Diagnostics.ProcessStartInfo
					{
						FileName = file,
						Arguments = args,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						CreateNoWindow = true
					}
				};

				process.Start();

				while (!process.StandardOutput.EndOfStream)
				{
					var line = process.StandardOutput.ReadLine();
					Console.WriteLine(line);
				}

				process.WaitForExit();
				return 0;
			}

			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return 1;
			}
		}

		public static int Main(string[] args)
		{
			var RootCommand = new RootCommand();
			RootCommand.Description = "psburn is tool to burn powershell scripts into powershell dependent executables " +
									  "by embedding it inside a C# program.";

			// Positional Arguments
			var PositionalArgument1 = new Option<string>(aliases: new string[] { "-i", "--input" },
														 description: "path to ps script");
			PositionalArgument1.IsRequired = true;
			RootCommand.Add(PositionalArgument1);

			// Optional Arguments
			RootCommand.Add(new Option<string>(aliases: new string[] { "-o", "--output" },
											   getDefaultValue: () => "Working Directory",
											   description: "path of output executable file"));

			RootCommand.Add(new Option<bool>(aliases: new string[] { "-d", "--debug" },
											 getDefaultValue: () => false,
											 description: "don't delete runtime generated files and C# program"));

			RootCommand.Add(new Option<bool>(aliases: new string[] { "-v", "--verbose" },
											 getDefaultValue: () => false,
											 description: "generate more outputs and logs than usual"));

			RootCommand.Add(new Option<string>(aliases: new string[] { "--icon" },
											   getDefaultValue: () => "No Icon",
											   description: "apply icon to generated executable"));

			RootCommand.Add(new Option<bool>(aliases: new string[] { "--noconsole" },
											 getDefaultValue: () => false,
											 description: "create executable without a console, this helps for running scripts in background"));

			RootCommand.Add(new Option<bool>(aliases: new string[] { "--uac-admin" },
											 getDefaultValue: () => false,
											 description: "this option creates a manifest which will request elevation upon application restart"));

			RootCommand.Add(new Option<string>(aliases: new string[] { "--executionpolicy" },
											   getDefaultValue: () => "Bypass",
											   description: "execution policy for the powershell script"));

			RootCommand.Add(new Option<string>(aliases: new string[] { "--cscpath" },
											   getDefaultValue: () => "Auto Detect",
											   description: "C# compiler path (C:\\Windows\\Microsoft.Net\\Framework\\<version>\\csc.exe)"));

			RootCommand.Handler = CommandHandler.Create<string, string, bool, bool, string, bool, bool, string, string>(
													   (input, output, debug, verbose, icon, noconsole, uacadmin, executionpolicy, cscpath) =>
													   {
														   MainActivity(input, output, debug, verbose, icon, noconsole, uacadmin, executionpolicy, cscpath);
													   });

			return RootCommand.InvokeAsync(args).Result;
		}

		public static void MainActivity(string input, string output, bool debug, bool verbose, string icon,
										bool noconsole, bool uacadmin, string executionpolicy, string cscpath)
		{
			// Verbose outputs
			if (verbose)
			{
				Console.WriteLine($"PS Script: {input}");
				Console.WriteLine($"Using Execution Policy: {executionpolicy}");
				Console.WriteLine($"Debugging: {debug}");
			}

			// Reading powershell script and finding base name
			string PSScriptFile = "";
			string[] PSScriptFileLines = System.IO.File.ReadAllLines(input);
			string FileBaseName = System.IO.Path.GetFileNameWithoutExtension(input);

			// Handling powershell script comments
			foreach (string line in PSScriptFileLines)
			{
				if (line.StartsWith("#")) { PSScriptFile += "\n"; }
				else if (line.EndsWith("\n")) { PSScriptFile += line; }
				else { PSScriptFile += line + "\n"; }
			}

			// Dumping a cs file from psboilerplate.cs
			string MainCsFile = $"{FileBaseName}.cs";
			string[] CompileCsLines = EmmededFileReadAllLines("psburn.assets.psboilerplate.cs");

			foreach (string line in CompileCsLines)
			{
				if (line.Contains("string ExPolicy"))
				{
					CompileCsLines[Array.IndexOf(CompileCsLines, line)] = $"\t\t\tstring ExPolicy = \"{executionpolicy}\";";
				}

				if (line.Contains("string PSScriptFile"))
				{
					CompileCsLines[Array.IndexOf(CompileCsLines, line)] = $"\t\t\tstring PSScriptFile = @\"{PSScriptFile}\";";
				}
			}

			System.IO.File.WriteAllLines(MainCsFile, CompileCsLines);

			// Try to auto detect csc.exe
			string cscexe;
			if (cscpath == "Auto Detect")
			{
				var cscpaths = System.IO.Directory.GetDirectories("C:\\Windows\\Microsoft.NET\\Framework");
				cscexe = System.IO.Path.Join(cscpaths[cscpaths.Length - 1], "csc.exe");
			}
			else { cscexe = cscpath; }

			// Managing csc args supplied by user
			string cscargs = "/nologo /optimize ";

			if (icon != "No Icon") { cscargs += $"/win32icon:\"{icon}\" "; }
			if (noconsole) { cscargs += "/target:winexe "; }
			if (uacadmin)
			{
				string[] ManifestFileContent = EmmededFileReadAllLines("psburn.assets.manifest.xml");
				System.IO.File.WriteAllLines("manifest.xml", ManifestFileContent);
				cscargs += $"/win32manifest:manifest.xml ";
			}
			if (output != "Working Directory") { cscargs += $"/out:\"{output}\" "; }

			cscargs += $"\"{MainCsFile}\" ";

			if (verbose)
			{
				Console.WriteLine($"Using C# Compiler: {cscexe}");
				Console.WriteLine($"Supplied C# Args: {cscargs}");
			}

			// Running windows inbuilt cs compiler to compile boiler genrated cs file
			int SubprocessStatus = RunSubprocess(cscexe, cscargs);

			if (debug == false)
			{
				if (uacadmin) { System.IO.File.Delete("manifest.xml"); }
				System.IO.File.Delete(MainCsFile);
			}

			if (verbose)
			{
				if (SubprocessStatus == 1) { Console.WriteLine("Result: Unsucessfull"); }
				else if (SubprocessStatus == 0) { Console.WriteLine("Result: Sucessfully"); }
			}

		}
	}
}
