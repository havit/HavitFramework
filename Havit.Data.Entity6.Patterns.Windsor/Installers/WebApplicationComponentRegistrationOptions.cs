using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using System;

namespace Havit.Data.Entity.Patterns.Windsor.Installers
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// Výchozí GeneralLifestyle je HybridPerWebRequestPerThread.
	/// </summary>
	[Obsolete("Používá lifestyle HybridPerWebRequestPerThread, který je díky \"PerThread\" nežádoucí - nefunguje ve scénářích s async/await, nefunguje v (background) Tasku, atp. "
		+ "Náhradou je použití lifestyle HybridPerWebRequestScoped z Havit.CastleWindsor.WebForms, nahradit tedy WebApplicationComponentRegistrationOptions za použití: new ComponentRegistrationOptions { GeneralLifestyle = lifestyle => lifestyle.HybridPerWebRequestScope() }. "
		+ "Pro resolvování mimo web request (při startu aplikace, v Tasku) je třeba založit scope (using (container.BeginScope())). "
		+ "Pokud v aplikaci není třeba resolvovat závislosti mimo web request (při startu aplikace, Tasky) je možné použít lifestyle PerWebRequest.")]
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
