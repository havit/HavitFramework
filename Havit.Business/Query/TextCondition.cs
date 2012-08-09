using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující textový øetìzec.
	/// </summary>
	public static class TextCondition
	{
		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na rovnost. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateEquals(PropertyInfo property, string value)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateLike(PropertyInfo property, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, property, ValueOperand.Create(GetLikeExpression(value)));
		}

		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value">
		/// Podporována hvìzdièková konvence takto:
		///		- pokud parametr neobsahuje hvìzdièku, hledá se LIKE parametr%
		///		- pokud parametr obsahuje hvìzdièku, zamìní se hvìzdièka za procento a hledá se LIKE parametr.
		///	Pokud parametr obsahuje speciální znaky pro operátor LIKE jako procento nebo podtržítko,
		///	jsou tyto znaky pøekódovány, takže nemají funkèní význam.
		/// </param>
		/// <example>
		///	Pø. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoè" protože _ nemá funkèní význam, ale najde "k_lo" i "k_olotoè".
		/// </example>
		public static Condition CreateWildcards(PropertyInfo property, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, property, ValueOperand.Create(GetWildCardsLikeExpression(value)));
		}

		#region GetLikeExpression, GetWildCardsLikeExpression
		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like.
		/// Nahrazuje % na [%] a _ na [_].
		/// </summary>
		public static string GetLikeExpression(string text)
		{
			string result;
			result = text.Trim().Replace("%", "[%]");
			result = result.Replace("_", "[_]");
			return result;
		}

		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like. 
		/// Navíc je vzat ohled na hvìzdièkovou konvenci.
		/// Nahrazuje % na [%] a _ na [_] a jako poslední zamìní * za %.
		/// Pøíklad "*text1%text2*text3" bude transformováno na "%text1[%]text2%text3".
		/// </summary>
		public static string GetWildCardsLikeExpression(string text)
		{
			string result;
			result = GetLikeExpression(text);

			if (result.Contains("*"))
			{
				result = result.Replace("*", "%");
			}
			else
			{
				result += "%";
			}

			return result;
		}
		#endregion
	
	}
}
