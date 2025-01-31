namespace Havit.Web.UI;

/// <summary>
/// Způsob serializace viewstate.
/// </summary>
/// <remarks>
/// 9.10.2012: BinaryFormatter přepínáme na LosFormatter.
/// 13.12.2019: LosFormatter přepínáme na BinaryFormatter, commit má v popisu "performance".
/// 26.10.2021: BinaryFormatter přepínáme na LosFormatter s možností nastavení BinaryFormatteru.
/// 
/// Komentář k 26.10.2021
/// Ověřujeme scénáře na DSV Commerce, serializovaná velikost ViewState 15kB, 90kB, 15MB.
/// Ve všech situacích vítězí LosFormatter pro serializaci i deserializaci.
/// Nedaří se nalézt situaci, kdy by byl BinaryFormatter efektivnější, byť předpokládám, vzhledem ke commitům a k popisům v navázaných timesheetech,
/// že jsme to v prosinci 2019 měřili a dospěli jsme k závěru, že je někde výhodnější. Nepodařilo se nalézt scénář, kdy by tomu tak bylo.
/// Nepodařilo se mi najít projekt, na kterém jsme to v roce 2019 ladili.
/// Teoreticky může být rozdíl způsoben tím, že měření v roce 2019 mohlo probíhat na .NETu 4.6.x,
/// avšak ani žádnou hypotézou okolo tohoto se nepodařilo najít scénář, kdy by byl BinaryFormatter rychlejší,
/// LosFormatter neobsahuje ve zdrojových kódech žádnou relevantní změnu.
/// Nyní ověřujeme rychlost na .NETu 4.8, bohatě stačí měření pomocí wall clock time (a Stopwatch).
/// </remarks>
public enum FileStoragePageStatePersisterSerializationStrategy
{
	/// <summary>
	/// Bude použit BinaryFormatter pro serializaci a deserializaci (a jako fallback pro deserializaci LosFormatter).
	/// Pozor při použití BinaryFormatteru na deserializi "velkých viewstate", BinaryFormatter obsahuje skrytou kvadratickou závislost.
	/// Existuje možnost, jak ji obejít, více viz https://aloiskraus.wordpress.com/2018/05/06/serialization-performance-update-with-net-4-7-2/
	/// </summary>
	BinaryFormatter,

	/// <summary>
	/// Bude použit LosFormatter pro serializaci a deserializaci (a jako fallback pro deserializaci BinaryFormatter). Výchozí varianta.
	/// </summary>
	LosFormatter
}
