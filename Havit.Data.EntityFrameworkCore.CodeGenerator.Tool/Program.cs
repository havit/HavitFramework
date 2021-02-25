using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("HAVIT Entity Framework Core CodeGenerator Tool");
			Console.WriteLine("----------------------------------------------");

			DirectoryInfo solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory);// @"D:\Dev\002.HFW-NewProjectTemplate";
			while (System.IO.Directory.GetFiles(solutionDirectory.FullName, "*.sln", SearchOption.TopDirectoryOnly).Length == 0)
			{
				if (solutionDirectory.Root.FullName == solutionDirectory.FullName)
				{
					Console.WriteLine("Solution file was not found.");
					return;
				}
				solutionDirectory = solutionDirectory.Parent;
			}

			string[] files = System.IO.Directory
				.GetFiles(Path.Combine(solutionDirectory.FullName, @"DataLayer\bin"), "*.Entity.dll", SearchOption.AllDirectories)
				.Where(file => !file.EndsWith(@"Havit.Entity.dll"))
				.Where(file => !file.Contains(@"ref\"))
				.OrderByDescending(item => System.IO.File.GetLastAccessTime(item))
				.ToArray();

			if (files.Length == 0)
			{
				Console.WriteLine("Assembly *.Entity.dll was not found.");
				return;
			}

			string applicationEntityAssembly = files.First();
			string workingFolder = System.IO.Path.GetDirectoryName(applicationEntityAssembly);
			Console.WriteLine($"Using folder {workingFolder}.");

			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly(workingFolder);
			Assembly assembly = Assembly.Load(new AssemblyName { Name = "Havit.Data.EntityFrameworkCore.CodeGenerator" });

			if (assembly == null)
			{
				Console.WriteLine("Assembly Havit.Data.EntityFrameworkCore.CodeGenerator could not be loaded.");
				return;
			}

			Type program = assembly.GetType("Havit.Data.EntityFrameworkCore.CodeGenerator.Program");
			if (program == null)
			{
				Console.WriteLine("CodeGenerator entry point (class) was not found.");
				return;
			}
			
			MethodInfo main = program.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
			if (program == null)
			{
				Console.WriteLine("CodeGenerator entry point (method) was not found.");
				return;
			}

			Console.WriteLine("Starting codegenerator...");
			main.Invoke(null, new object[] { new string[] { solutionDirectory.FullName, System.IO.Path.GetFileNameWithoutExtension(applicationEntityAssembly) } });
		}

		private static ResolveEventHandler ResolveAssembly(string appBasePath)
		{
			return (object sender, ResolveEventArgs args) =>
			{
				var assemblyName = new AssemblyName(args.Name);

				foreach (var extension in new[] { ".dll", ".exe" })
				{
					var path = Path.Combine(appBasePath, assemblyName.Name + extension);
					if (File.Exists(path))
					{
						try
						{
							return Assembly.LoadFrom(path);
						}
						catch
						{
							// NOOP
						}
					}
				}
				return null;
			};
		}
	}
}