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

		/// <summary>
		/// Vytvoøí podmínku testující øetìzec na podobnost operátorem LIKE. Citlivost na velká a malá písmena, diakritiku apod. vychází z nastavení serveru.
		/// </summary>
		public static Condition CreateLike(IOperand operand, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(GetLikeExpression(value)));
		}

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
		///	Pø. Hledání výrazu "k_lo*" nenajde "kolo" ani "kolotoè" protože _ nemá funkèní význam, ale najde "k_lo" i "k_olotoè".
		/// </example>
		public static Condition CreateWildcards(IOperand operand, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, operand, ValueOperand.Create(GetWildCardsLikeExpression(value)));
		}

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


		#region GetLikeExpression, GetWildCardsLikeExpression
		/// <summary>
		/// Transformuje øetìzec naøetìzec, který je možné použít jako hodnota k operátoru like.
		/// Nahrazuje % na [%] a _ na [_].
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
