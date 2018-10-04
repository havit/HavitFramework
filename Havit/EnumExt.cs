using System;

namespace Havit
{
	/// <summary>
	/// Poskytuje metody týkající se základního výčtového typu System.Enum.
	/// </summary>
	/// <remarks>
	/// Třída samostná není potomkem System.Enum, protože ze System.Enum nelze dědit.
	/// </remarks>
	public static class EnumExt
	{
		/// <summary>
		/// Vrátí hodnotu atributu [Description("...")] určité hodnoty zadaného výčtového typu.
		/// </summary>
		/// <param name="enumType">výčtový typ</param>
		/// <param name="hodnota">hodnota, jejíž Description chceme</param>
		/// <returns>hodnota atributu [Description("...")]</returns>
		/// <remarks>Není-li atribut Description definován, vrátí prázdný řetězec.</remarks>
		/// <example>
		///	<code>
		/// using System.ComponentModel;<br/>
		/// <br/>
		/// public enum Barvy<br/>
		/// {<br/>
		///		[Description("červená")]<br/>
		///		Cervena,<br/>
		///	<br/>
		///		[Description("modrá")]<br/>
		///		Modra<br/>
		///	}<br/>
		///	</code>
		/// </example>
		public static string GetDescription(Type enumType, object hodnota)
		{
			string strRet = "";

			try
			{
				System.Reflection.FieldInfo objInfo =
					enumType.GetField(System.Enum.GetName(enumType, hodnota));

				System.ComponentModel.DescriptionAttribute objDescription =
					(System.ComponentModel.DescriptionAttribute)objInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)[0];

				strRet = objDescription.Description;
			}
			catch (Exception)
			{
				// chybí description
			}

			return strRet;
		}
	}
}
