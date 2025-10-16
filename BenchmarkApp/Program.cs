using BenchmarkDotNet.Running;

namespace Havit.EFCoreTests.BenchmarkApp;

public static class Program
{
	public static void Main()
	{
		_ = BenchmarkRunner.Run(typeof(Program).Assembly);
	}
}
