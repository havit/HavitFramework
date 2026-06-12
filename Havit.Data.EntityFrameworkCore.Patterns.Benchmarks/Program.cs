using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Havit.Data.EntityFrameworkCore.Patterns.Benchmarks;

public static class Program
{
	public static void Main(string[] args)
	{
		// In-process toolchain: výchozí (out-of-process) toolchain vnucuje všem projektům společný výstupní adresář,
		// ve kterém si multi-target závislosti (Havit.Core) přepisují výstupy, a build vygenerovaného projektu pak selhává.
		IConfig config = ManualConfig.Create(DefaultConfig.Instance)
			.AddJob(Job.ShortRun
				.WithToolchain(InProcessEmitToolchain.Instance)
				.AsDefault());

		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
	}
}
