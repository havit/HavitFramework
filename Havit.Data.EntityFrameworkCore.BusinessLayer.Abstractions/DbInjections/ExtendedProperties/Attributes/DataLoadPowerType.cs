namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	/// <summary>
	/// Určuje, jaké množství dat se vrací z uložené procedury. Je použito pro návratové typy Object a Collection (viz ResultType).
	/// </summary>
	public enum DataLoadPowerType
	{
		/// <summary>
		/// Datový zdroj obsahuje jen informace pro založení ghosta (primární klíč).
		/// </summary>
		Ghost,
		
		/// <summary>
		/// Datový zdroj obsahuje nekompletní řádek dat.
		/// </summary>
		PartialLoad,

		/// <summary>
		/// Datový zdroj obsahuje kompletní řádek dat (všechny možné sloupce).
		/// </summary>
		FullLoad
	}
}