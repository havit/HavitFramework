using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer cachovací strategie.
	/// </summary>
	public interface ICachingInstaller
	{
		/// <summary>
		/// Zaregistruje služby.
		/// </summary>
		public void Install(IServiceCollection serviceCollection);
	}
}
