using System;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	public class ServiceCollectionComponentRegistrationOptions : ComponentRegistrationOptionsBase<ServiceLifetime>
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		internal ServiceCollectionComponentRegistrationOptions()
		{
			this.GeneralLifestyle = ServiceLifetime.Scoped;
		}
	}
}
