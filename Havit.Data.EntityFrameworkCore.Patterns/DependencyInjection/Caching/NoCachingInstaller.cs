using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer, která zaregistruje službu, která nic necachuje (NoCachingEntityCacheManager). 
	/// </summary>
	public sealed class NoCachingInstaller : ICachingInstaller
	{
		/// <inheritdoc />
		public void Install(IServiceInstaller serviceInstaller)
		{
			serviceInstaller.AddServiceSingleton<IEntityCacheManager, NoCachingEntityCacheManager>();
		}
	}
}
