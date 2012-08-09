using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business.Conditions;

namespace Havit.Business.Conditions
{
	public static class TextCondition
	{
		public static ICondition CreateEquals(Property property, string value)
		{
			return new BinaryCondition(BinaryCondition.EqualsPattern, property, ValueOperand.FromString(value));
		}

		public static ICondition CreateLike(Property property, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, property, ValueOperand.FromString(GetLikeExpression(value)));
		}

		public static ICondition CreateWildcards(Property property, string value)
		{
			return new BinaryCondition(BinaryCondition.LikePattern, property, ValueOperand.FromString(GetWildCardsLikeExpression(value)));
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
				result = result.Replace("*", "%");
			else
				result += "%";

			return result;
		}
		#endregion
	
	}
}
