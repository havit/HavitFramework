using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.EFCoreTests.Entity;
using Havit.Services;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Havit.Data.Patterns.DataLoaders;
using Havit.EFCoreTests.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using System.Transactions;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.EFCoreTests.DataLayer.Seeds.Core;
using System.Data.SqlClient;
using Havit.Data.Patterns.Exceptions;
using Havit.EFCoreTests.DataLayer.Repositories;
using System.Linq.Expressions;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.DataSources;
using Havit.EFCoreTests.DataLayer.Seeds.ProtectedProperties;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using BenchmarkDotNet.Running;

namespace Havit.EFCoreTests.BenchmarkApp
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			// https://benchmarkdotnet.org/articles/guides/how-to-run.html

			// Generuje výsledky ve slořce bin (Release).
			// A to včetně výsledku profillingu (*.speedscope a *.nettrace).

			#region Release only, no debugger
#if DEBUG
			throw new InvalidOperationException("Benchmarky by měly být spuštěny v Release mode.");

#endif
			if (Debugger.IsAttached)
            {
				throw new InvalidOperationException("Benchmarky by měly být spuštěny bez debuggeru.");
			}
            #endregion

            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}
