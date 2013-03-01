using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Jednoduchý skládač řetězce. 
    /// Vnitřně používá StringBuilder, na rozdíl od něj umožňuje text
    /// jen přidávat, neumožňuje jej modifikovat.
    /// </summary>
	public class ScriptBuilder
	{
		#region Private fields
		private StringBuilder builder = new StringBuilder();
		#endregion

		#region IsEmpty
		/// <summary>
		/// Vrací true, ScriptBuilder neobsahuje žádný text. Jinak false.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return builder.Length == 0;
			}
		}
		#endregion

		#region Append, AppendLine, AppendFormat
		/// <summary>
		/// Připojí řetězec na konec textu.
		/// </summary>
		/// <param name="value">Přidávaná hodnota.</param>
		public void Append(string value)
		{
			builder.Append(value);
		}
		
		/// <summary>
		/// Připojí řetězec na konec textu a vloží symbol konce řádku (Environment.NewLine).
		/// </summary>
		/// <param name="value">Přidávaná hodnota.</param>
		public void AppendLine(string value)
		{
			this.Append(value);
			builder.Append(Environment.NewLine);
		}

		/// <summary>
		/// Po zformátování připojí řetězec na konec textu.
		/// </summary>
		/// <param name="value">Šablona přidávané hodnota.</param>
		/// <param name="parameters">Parametry šablony.</param>
		public void AppendFormat(string value, params object[] parameters)
		{
			builder.AppendFormat(value, parameters);
		}
		
		/// <summary>
		/// Po zformátování připojí řetězec na konec textu a doplní jej symbolem pro konec řádky.
		/// </summary>
		/// <param name="value">Šablona přidávané hodnota.</param>
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
