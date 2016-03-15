using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace Havit.Data.Entity.Patterns.Windsor.Installers
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// Výchozí GeneralLifestyle je PerWcfSession.
	/// </summary>
	public class WcfComponentRegistrationOptions : ComponentRegistrationOptions
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public WcfComponentRegistrationOptions()
		{
			GeneralLifestyle = (LifestyleGroup<object> group) => group.PerWcfSession();
		}
	}
}
