using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.Repositories;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.EFCoreTests.BenchmarkApp
{
    [EventPipeProfiler(EventPipeProfile.CpuSampling)]
    public class BenchmarkTask1
    {
        [Benchmark]
        public void BenchmarkFalse()
        {
            CreateServiceProvider(false);
        }

        [Benchmark]
        public void BenchmarkTrue()
        {
            CreateServiceProvider(true);
        }

        private static void CreateServiceProvider(bool validate)
        {
            IServiceCollection services = new ServiceCollection();
            services.WithEntityPatternsInstaller()
                .AddDataLayer(typeof(IPersonRepository).Assembly)
                .AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(optionsBuilder => optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0"))
                .AddEntityPatterns()
                .AddLookupService<IUserLookupService, UserLookupService>();

            services.AddSingleton<ITimeService, ServerTimeService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
            services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = validate
            }).GetRequiredService<IDbContext>();
        }
    }
}
