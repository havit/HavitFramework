using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínky testující textový řetězec.
	/// </summary>
	public static class TextCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoří podmínku testující řetězec na rovnost. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, string value)
		{
			return CreateEquals(operand, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující rovnost dvou operandů. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, operand1, operand2);
		}
		#endregion

		#region CreateLike
		/// <summary>
		/// Vytvoří podmínku testující řetězec na podobnost operátorem LIKE. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateLike(IOperand operand, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(value));
		}
		#endregion

		#region CreateWildcards
		/// <summary>
		/// Vytvoří podmínku testující řetězec na podobnost operátorem LIKE.
		/// </summary>
		/// <param name="value">
		/// Podporována hvězdičková konvence takto:
		///		- pokud parametr neobsahuje hvězdičku, hledá se LIKE parametr%
		///		- pokud parametr obsahuje hvězdičku, zamění se hvězdička za procento a hledá se LIKE parametr.
		///	Pokud parametr obsahuje speciální znaky pro operátor LIKE jako procento nebo podtržítko,
		///	jsou tyto znaky překódovány, takže nemají funkční význam.
		/// </param>
		/// <example>
		///	Př. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoč" protože _ nemá funkční význam, ale najde "k_lo" i "k_lotoč".
		/// </example>
		public static Condition CreateWildcards(IOperand operand, string value)
		{
			return CreateWildcards(operand, value, WildCardsLikeExpressionMode.StartsWith);
		}

		/// <summary>
		/// Vytvoří podmínku testující řetězec na podobnost operátorem LIKE.
		/// </summary>
		/// <param name="value">
		/// Podporována hvězdičková konvence takto:
		///		- pokud parametr neobsahuje hvězdičku, hledá se pomocí LIKE parametr% nebo LIKE %parametr% podle parametru wildCardsLikeExpressionMode.
		///		- pokud parametr obsahuje hvězdičku, zamění se hvězdička za procento a hledá se LIKE parametr.
		///	Pokud parametr obsahuje speciální znaky pro operátor LIKE jako procento nebo podtržítko,
		///	jsou tyto znaky překódovány, takže nemají funkční význam.
		/// </param>
		/// <param name="wildCardsLikeExpressionMode">Režim použitý v případě, že hodnota value neobsahuje speciální znak (hvězdičku či otazník).</param>
		/// <example>
		///	Př. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoč" protože _ nemá funkční význam, ale najde "k_lo" i "k_olotoč".
		/// </example>
		public static Condition CreateWildcards(IOperand operand, string value, WildCardsLikeExpressionMode wildCardsLikeExpressionMode)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(GetWildCardsLikeExpression(value, wildCardsLikeExpressionMode)));
		}
		#endregion

		#region Create
		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand, ComparisonOperator comparisonOperator, string value)
		{
			return Create(operand, comparisonOperator, ValueOperand.Create(value));
		}

		/// <summary>
		/// Vytvoří podmínku testující hodnoty pomocí zadaného operátoru.
		/// </summary>
		public static Condition Create(IOperand operand1, ComparisonOperator comparisonOperator, IOperand operand2)
		{
			return new BinaryCondition(operand1, BinaryCondition.GetComparisonPattern(comparisonOperator), operand2);
		}
		#endregion

		#region CreateIsNullOrEmpty, CreateIsNotNullOrEmpty
		/// <summary>
		/// Vytvoří podmínku testující řetězec na prázdnou hodnotu - null nebo empty.
		/// </summary>
		public static Condition CreateIsNullOrEmpty(IOperand operand)
		{
			return OrCondition.Create(
				NullCondition.CreateIsNull(operand),
				TextCondition.CreateEquals(operand, string.Empty)
			);
		}

		/// <summary>
		/// Vytvoří podmínku testující řetězec na ne prázdnou hodnotu - ani null ani empty.
		/// </summary>
		public static Condition CreateIsNotNullOrEmpty(IOperand operand)
		{
			return AndCondition.Create(
				NullCondition.CreateIsNotNull(operand),
				TextCondition.Create(operand, ComparisonOperator.NotEquals, string.Empty)
			);
		}
		#endregion
		
		#region GetLikeExpression, GetWildCardsLikeExpression
		/// <summary>
		/// Transformuje řetězec nařetězec, který je možné použít jako hodnota k operátoru like. Tj. nahrazuje % na [%] a _ na [_].
		/// Nepřidává % na konec, to dělá GetWildCardsLikeExpression().
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
		/// Transformuje řetězec nařetězec, který je možné použít jako hodnota k operátoru like. 
		/// Navíc je vzat ohled na hvězdičkovou konvenci našeho standardního UI (pokud výraz neobsahuje wildcards, přidá hvězdičku na konec).
		/// (Nahrazuje % na [%] a _ na [_] a jako poslední zamění * za %, resp. přidá % nakonec, pokud wildcards nebyly použity.)
		/// Příklad "*text1%text2*text3" bude transformováno na "%text1[%]text2%text3".
		/// </summary>
		public static string GetWildCardsLikeExpression(string text)
		{
			return GetWildCardsLikeExpression(text, WildCardsLikeExpressionMode.StartsWith);
		}

		/// <summary>
		/// Transformuje řetězec nařetězec, který je možné použít jako hodnota k operátoru like. 
		/// Navíc je vzat ohled na hvězdičkovou konvenci podle parametru wildCardsLikeExpressionMode (pokud nejsou použity wildcards, doplní % na konec (StartsWith) nebo na začátek i na konec (Contains) ).
		/// Příklad "*text1%text2*text3" bude transformováno na "%text1[%]text2%text3".
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
