using BenchmarkDotNet.Running;

namespace Havit.EFCoreTests.BenchmarkApp;

public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
	}
}
