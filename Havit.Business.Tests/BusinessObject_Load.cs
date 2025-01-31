using Havit.BusinessLayerTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.Tests;

[TestClass]
public class BusinessObject_Load
{
	/// <summary>
	/// Ověřovaný scénář 1:
	/// Na cachovaném readonly objektu v cache se pustí:
	/// * Init (thread 1) - výsledkem je objekt s IsLoaded = false
	/// * Load (thread 1) - výsledkem je objekt s IsLoaded = true
	/// * Init (thread 2) - výsledkem je objekt s IsLoaded = true, avšak hodnoty díky zavolání Init nemá nastaveny
	/// * Load (thread 2) - již nenastavuje hodnoty, protoe objekt je IsLoaded
	/// 
	/// Ověřovaný scénář 2:
	/// * Init (thread 1) - výsledkem je objekt s IsLoaded = false
	/// * Load (thread 1) - výsledkem je objekt s IsLoaded = true
	/// * Init (thread 2) - výsledkem je objekt s IsLoaded = true, avšak hodnoty díky zavolání Init nemá nastaveny
	/// * AnyPropertyHolder.Value (thread 1) - selže, protože objekt je IsLoaded (takže EnsureLoaded nic neudělá), ale objekty nemá nastaveny
	/// * Load (thread 2)
	/// 
	/// Rozdíl scénářů 1 a 2:
	/// Po scénáři 1 zůstane v cache nabořený objekt, který není možné používat, neboť při šahnutí na vlastnost objektu vyhazuje výjimku.
	/// Scénář 2 toto neříká, teoreticky může thread 2 objekt ještě opravit. Snažíme se však eliminovat i tyto výpadky.
	/// </summary>
	[TestMethod]
	[Ignore] // runtime > 10 sekund
	public void BusinessObject_Load_DataRecord_CheckParallel()
	{
		// Problém se dařilo zreprodukovat během cca 2-3 iterací.
		// Nikdy se neprojevilo při spuštění Debug unit testů, jen při Run!
		for (int i = 0; i < 1000; i++)
		{
			// vyčistíme cache, abychom v jednotlivých bězích od sebe izolovali
			Havit.Web.HttpServerUtilityExt.ClearCache();

			Parallel.For(0, 2, _ => // pustíme dvakrát vedle sebe
			{
				using (new IdentityMapScope())
				{
					var roleLocalization = RoleLocalization.GetObject(1);
					RoleLocalizationCollection collection = new RoleLocalizationCollection { roleLocalization };

					// Act
					// pod pokličkou zavolá RoleLocalization.Load(DataRecord), jejíž paralelní běh ověřujeme
					collection.LoadAll();

					// Assert
					Assert.IsTrue(roleLocalization.IsLoaded); // s tímto nebyl nikdy problém, jen chceme mít objekt načtený

					// pokud je objekt ve stavu IsLoaded, ale nemá hodnoty property holderů,
					// vyhazuje výjimku InvalidOperationException (Hodnota nebyla inicializována.),
					// více viz PropertyHolderBase.CheckInitialization.
					Assert.IsFalse(String.IsNullOrEmpty(roleLocalization.Nazev));
				}
			});
		}
	}
}
