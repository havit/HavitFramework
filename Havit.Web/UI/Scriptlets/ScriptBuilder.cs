using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Jednoduchý skládaè øetìzce. 
    /// Vnitønì používá StringBuilder, na rozdíl od nìj umožòuje text
    /// jen pøidávat, neumožòuje jej modifikovat.
    /// </summary>
	public class ScriptBuilder
	{
		private StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Pøipojí øetìzec na konec textu.
        /// </summary>
        /// <param name="value">Pøidávaná hodnota.</param>
		public void Append(string value)
		{
			builder.Append(value);		
		}

        /// <summary>
        /// Po zformátování pøipojí øetìzec na konec textu.
        /// </summary>
        /// <param name="value">Šablona pøidávané hodnota.</param>
        /// <param name="parameters">Parametry šablony.</param>
		public void AppendFormat(string value, params object[] parameters)
		{
			builder.AppendFormat(value, parameters);
		}

        /// <summary>
        /// Vrátí složený text.
        /// </summary>
        /// <returns>Složený text.</returns>
		public override string ToString()
		{
			return builder.ToString();
		}
	}
}
