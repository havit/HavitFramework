using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer cachovací strategie.
	/// </summary>
	public interface ICachingInstaller<TLifetime>
	{
		/// <summary>
		/// Zaregistruje služby.
		/// </summary>
		public void Install(IServiceInstaller<TLifetime> serviceInstaller);
	}
}
