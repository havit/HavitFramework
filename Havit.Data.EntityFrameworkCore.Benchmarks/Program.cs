using BenchmarkDotNet.Running;

namespace Havit.Data.EntityFrameworkCore.Benchmarks;

public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
	}
}
