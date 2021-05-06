using System;


namespace RunProcess
{
	internal class Program
	{
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

		public static void Main(string[] args)
		{
			string PSScriptRoot = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			string PSScriptName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
			string ExPolicy = "Bypass";
			string PSScriptFile = "Write-Output 'Hello World!'";
			string PSEmbedString = String.Format("$PSScriptRoot = '{0}'\n\n", PSScriptRoot);
			bool CatFile = false;

			foreach (string argument in args)
			{
				if (argument == "-h")
                {
					Console.WriteLine(String.Format("usage: {0} [-h] [-cat] [-<var> ...]\n", PSScriptName));
					Console.WriteLine("dynamically auto compiled embedded powershell command script.\n");
					Console.Write("Note: you can access dynamic variables by setting them from command line and access them in powershell script by specified $ sign. ");
					Console.WriteLine("run example for better understanding.\n");
					Console.WriteLine("optional arguments:\n  -cat\t\t\tbefore executing powershell script cat it into console");
					Console.WriteLine("  -h\t\t\tshow this help message and exit\n");
					Console.WriteLine(String.Format("examples:\n\t{0} -cat", PSScriptName));
					Console.WriteLine(String.Format("\t{0} -cat -FirstName 360 -LastName modder -AnyName \"Means Any Thing\"", PSScriptName));
					Environment.Exit(0);
                }

				else if (argument == "-cat") { CatFile = true; }
				else if (argument.Contains("-")) { PSEmbedString += "$" + argument.Split('-')[1] + " = "; }
				else
				{
					PSEmbedString += String.Format("'{0}'\n", argument);
				}
			}

			PSEmbedString += "\n" + PSScriptFile;
			if (CatFile) { Console.WriteLine(PSEmbedString); }

			RunSubprocess("powershell.exe", String.Format("-ExecutionPolicy {0} -Command {1}", ExPolicy, PSEmbedString));
		}
	}
}
