using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace Havit.Data.Entity.Patterns.Windsor.Installers
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// Výchozí GeneralLifestyle je HybridPerWebRequestPerThread.
	/// </summary>
	public class WebApplicationComponentRegistrationOptions : ComponentRegistrationOptions
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public WebApplicationComponentRegistrationOptions()
		{
			GeneralLifestyle = (LifestyleGroup<object> group) => group.HybridPerWebRequestPerThread();
		}
	}
}
