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
		#region Private fields
		private StringBuilder builder = new StringBuilder();
		#endregion

		#region Append, AppendLine, AppendFormat
		/// <summary>
		/// Pøipojí øetìzec na konec textu.
		/// </summary>
		/// <param name="value">Pøidávaná hodnota.</param>
		public void Append(string value)
		{
			builder.Append(value);
		}
		
		/// <summary>
		/// Pøipojí øetìzec na konec textu a vloží symbol konce øádku (Environment.NewLine).
		/// </summary>
		/// <param name="value">Pøidávaná hodnota.</param>
		public void AppendLine(string value)
		{
			this.Append(value);
			builder.Append(Environment.NewLine);
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
		/// Po zformátování pøipojí øetìzec na konec textu a doplní jej symbolem pro konec øádky.
		/// </summary>
		/// <param name="value">Šablona pøidávané hodnota.</param>
		/// <param name="parameters">Parametry šablony.</param>
		public void AppendLineFormat(string value, params object[] parameters)
		{
			this.AppendFormat(value, parameters);
			builder.Append(Environment.NewLine);
		}
		#endregion

		#region ToString
		/// <summary>
        /// Vrátí složený text.
        /// </summary>
        /// <returns>Složený text.</returns>
		public override string ToString()
		{
			return builder.ToString();
		}
		#endregion
	}
}
