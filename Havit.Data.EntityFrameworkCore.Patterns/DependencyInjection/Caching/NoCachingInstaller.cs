using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Havit.Services.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer, která zaregistruje službu, která nic necachuje (NoCachingEntityCacheManager). 
	/// </summary>
	public sealed class NoCachingInstaller : ICachingInstaller
	{
		/// <inheritdoc />
		public void Install(IServiceCollection services)
		{
			services.AddSingleton<IEntityCacheManager, NoCachingEntityCacheManager>();
		}
	}
}
