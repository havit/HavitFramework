using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváøí podmínky testující textový øetìzec.
	/// </summary>
	public static class TextCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na rovnost. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, string value)
		{
			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující rovnost dvou operandù. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, operand1, operand2);
		}
		#endregion

		#region CreateLike
		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateLike(IOperand operand, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(value));
		}
		#endregion

		#region CreateWildcards
		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE.
		/// </summary>
		/// <param name="operand"></param>
		/// <param name="value">
		/// Podporována hvìzdièková konvence takto:
		///		- pokud parametr neobsahuje hvìzdièku, hledá se LIKE parametr%
		///		- pokud parametr obsahuje hvìzdièku, zamìní se hvìzdièka za procento a hledá se LIKE parametr.
		///	Pokud parametr obsahuje speciální znaky pro operátor LIKE jako procento nebo podtržítko,
		///	jsou tyto znaky pøekódovány, takže nemají funkèní význam.
		/// </param>
		/// <example>
		///	Pø. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoè" protože _ nemá funkèní význam, ale najde "k_lo" i "k_lotoè".
		/// </example>
		public static Condition CreateWildcards(IOperand operand, string value)
		{
			return CreateWildcards(operand, value, WildCardsLikeExpressionMode.StartsWith);
		}

		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE.
		/// </summary>
		/// <param name="operand"></param>
		/// <param name="value">
		/// Podporována hvìzdièková konvence takto:
		///		- pokud parametr neobsahuje hvìzdièku, hledá se pomocí LIKE parametr% nebo LIKE %parametr% podle parametru wildCardsLikeExpressionMode.
		///		- pokud parametr obsahuje hvìzdièku, zamìní se hvìzdièka za procento a hledá se LIKE parametr.
		///	Pokud parametr obsahuje speciální znaky pro operátor LIKE jako procento nebo podtržítko,
		///	jsou tyto znaky pøekódovány, takže nemají funkèní význam.
		/// </param>
		/// <example>
		///	Pø. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoè" protože _ nemá funkèní význam, ale najde "k_lo" i "k_olotoè".
		/// </example>
		public static Condition CreateWildcards(IOperand operand, string value, WildCardsLikeExpressionMode wildCardsLikeExpressionMode)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(GetWildCardsLikeExpression(value, wildCardsLikeExpressionMode)));
		}
		#endregion

		#region Create
		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, string value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoøí podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		}
		#endregion

		#region CreateIsNullOrEmpty, CreateIsNotNullNorEmpty
		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na prázdnou hodnotu - null nebo empty.
		/// </summary>
		public static Condition CreateIsNullOrEmpty(IOperand operand)
		{
			return OrCondition.Create(
				NullCondition.CreateIsNull(operand),
				TextCondition.CreateEquals(operand, string.Empty)
			);
		}

		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na ne prázdnou hodnotu - ani null ani empty.
		/// </summary>
		public static Condition CreateIsNotNullNorEmpty(IOperand operand)
		{
			return AndCondition.Create(
				NullCondition.CreateIsNotNull(operand),
				TextCondition.Create(operand, ComparisonOperator.NotEquals, string.Empty)
			);
		}
		#endregion
		
		#region GetLikeExpression, GetWildCardsLikeExpression
		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like. Tj. nahrazuje % na [%] a _ na [_].
		/// Nepøidává % na konec, to dìlá GetWildCardsLikeExpression().
		/// </summary>
		public static string GetLikeExpression(string text)
		{
			if (String.IsNullOrEmpty(text))
			{
				throw new ArgumentException("Argument text nesmí být null ani prázdný.", "text");
			}

			string result;
			result = text.Trim().Replace("%", "[%]");
			result = result.Replace("_", "[_]");
			return result;
		}

		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like. 
		/// Navíc je vzat ohled na hvìzdièkovou konvenci našeho standardního UI (pokud výraz neobsahuje wildcards, pøidá hvìzdièku na konec).
		/// (Nahrazuje % na [%] a _ na [_] a jako poslední zamìní * za %, resp. pøidá % nakonec, pokud wildcards nebyly použity.)
		/// Pøíklad "*text1%text2*text3" bude transformováno na "%text1[%]text2%text3".
		/// </summary>
		public static string GetWildCardsLikeExpression(string text)
		{
			return GetWildCardsLikeExpression(text, WildCardsLikeExpressionMode.StartsWith);
		}

		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like. 
		/// Navíc je vzat ohled na hvìzdièkovou konvenci podle parametru wildCardsLikeExpressionMode (pokud nejsou použity wildcards, doplní % na konec (StartsWith) nebo na zaèátek i na konec (Contains) ).
		/// Pøíklad "*text1%text2*text3" bude transformováno na "%text1[%]text2%text3".
		/// </summary>
		public static string GetWildCardsLikeExpression(string text, WildCardsLikeExpressionMode wildCardsLikeExpressionMode)
		{
			string result;
			result = GetLikeExpression(text);

			if (result.Contains("*"))
			{
				result = result.Replace("*", "%");
			}
			else
			{
				if (wildCardsLikeExpressionMode == WildCardsLikeExpressionMode.StartsWith)
				{
					result += "%";
				}

				if (wildCardsLikeExpressionMode == WildCardsLikeExpressionMode.Contains)
				{
					result = "%" + result + "%";
				}
			}

			return result;
		}
		#endregion

	}
}
