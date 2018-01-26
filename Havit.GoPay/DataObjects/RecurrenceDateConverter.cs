using Newtonsoft.Json.Converters;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Konvertuje DateTime do formátu "yyyy-MM-dd" pro potřeby nastavení do kdy je platná opakovatelnost platby 
	/// </summary>
	internal class RecurrenceDateConverter : IsoDateTimeConverter
	{
		public RecurrenceDateConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}