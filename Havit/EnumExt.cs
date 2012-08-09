using System;

namespace Havit
{
	/// <summary>
	/// Poskytuje metody týkající se základního výètového typu System.Enum.
	/// </summary>
	/// <remarks>
	/// Tøída samostná není potomkem System.Enum, protože ze System.Enum nelze dìdit.
	/// </remarks>
	public sealed class EnumExt
	{
		/// <summary>
		/// Vrátí hodnotu atributu [Description("...")] urèité hodnoty zadaného výètového typu.
		/// </summary>
		/// <param name="enumType">výètový typ</param>
		/// <param name="hodnota">hodnota, jejíž Description chceme</param>
		/// <returns>hodnota atributu [Description("...")]</returns>
		/// <remarks>Není-li atribut Description definován, vrátí prázdný øetìzec.</remarks>
		/// <example>
		///	<code>
		/// using System.ComponentModel;
		/// 
		/// public enum Barvy
		/// {
		///		[Description("èervená")]
		///		Cervena,
		///		
		///		[Description("modrá")]
		///		Modra
		///	}
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
			catch(Exception)
			{
				// chybí description
			}

			return strRet;
		}

		
		/// <summary>
		/// Prázdný private constructor zamezující vytvoøení instance tøídy.
		/// </summary>
		private EnumExt() {}
	}
}
