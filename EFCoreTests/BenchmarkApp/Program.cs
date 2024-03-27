using BenchmarkDotNet.Running;

namespace Havit.EFCoreTests.BenchmarkApp;

public static class Program
{
	public static void Main(string[] args)
	{
		_ = BenchmarkRunner.Run(typeof(Program).Assembly);
	}
}
