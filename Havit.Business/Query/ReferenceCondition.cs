using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Vytváří podmínku testující referenční hodnotu (cizí klíč).
	/// </summary>
	public static class ReferenceCondition
	{
		#region CreateEquals
		/// <summary>
		/// Vytvoří podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, int? id)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			if (id == BusinessObjectBase.NoID)
			{
				throw new ArgumentException("ID objektu nesmí mít hodnotu BusinessObjectBase.NoID.", "id");
			}

			if (id == null)
			{
				return NullCondition.CreateIsNull(operand);
			}
			else
			{
				return NumberCondition.CreateEquals(operand, id.Value);
			}
		}

		/// <summary>
		/// Vytvoří podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand, BusinessObjectBase businessObject)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			if (businessObject == null)
			{
				return CreateEquals(operand, (int?)null);
			}

			if (businessObject.IsNew)
			{
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");
			}

			return CreateEquals(operand, businessObject.ID);
		}

		/// <summary>
		/// Vytvoří podmínku na rovnost reference.
		/// </summary>
		public static Condition CreateEquals(IOperand operand1, IOperand operand2)
		{
			Contract.Requires<ArgumentNullException>(operand1 != null, nameof(operand1));
			Contract.Requires<ArgumentNullException>(operand2 != null, nameof(operand2));

			return NumberCondition.CreateEquals(operand1, operand2);
		}
		
		#endregion

		#region CreateNotEquals
		/// <summary>
		/// Vytvoří podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand, int? id)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			if (id == BusinessObjectBase.NoID)
			{
				throw new ArgumentException("ID objektu nesmí mít hodnotu BusinessObjectBase.NoID.", "id");
			}

			if (id == null)
			{
				return NullCondition.CreateIsNotNull(operand);
			}
			else
			{
				return NumberCondition.Create(operand, ComparisonOperator.NotEquals, id.Value);
			}
		}

		/// <summary>
		/// Vytvoří podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand, BusinessObjectBase businessObject)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));

			if (businessObject == null)
			{
				return CreateNotEquals(operand, (int?)null);
			}

			if (businessObject.IsNew)
			{
				throw new ArgumentException("Nelze vyhledávat podle nového neuloženého objektu.", "businessObject");
			}

			return CreateNotEquals(operand, businessObject.ID);
		}

		/// <summary>
		/// Vytvoří podmínku na nerovnost reference.
		/// </summary>
		public static Condition CreateNotEquals(IOperand operand1, IOperand operand2)
		{
			Contract.Requires<ArgumentNullException>(operand1 != null, nameof(operand1));
			Contract.Requires<ArgumentNullException>(operand2 != null, nameof(operand2));

			return NumberCondition.Create(operand1, ComparisonOperator.NotEquals, operand2);
		}
		
		#endregion

		#region CreateIn
		/// <summary>
		/// Vytvoří podmínku existence hodnoty v poli ID objektů.
		/// </summary>
		/// <param name="operand">Testovaný operand.</param>
		/// <param name="ids">Seznam hodnot. Ověřue se, že hodnota operandu je mezi těmito hodnotami.</param>
		/// <returns>Podmínka testující existenci hodnoty v poli ID objektů.</returns>
		public static Condition CreateIn(IOperand operand, int[] ids)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));
			Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));

			return NumberCondition.CreateIn(operand, ids);
		}

		/// <summary>
		/// Vytvoří podmínku existence hodnoty v poli ID objektů.
		/// </summary>
		/// <param name="operand">Testovaný operand.</param>
		/// <param name="ids">Seznam hodnot. Ověřue se, že hodnota operandu je mezi těmito hodnotami.</param>
		/// <param name="mode">Způsob tvoření dotazu.</param>
		/// <returns>Podmínka testující existenci hodnoty v poli ID objektů.</returns>
		[Obsolete("Parameter mode byl zrušen a neuvažuje se.", true)]
		public static Condition CreateIn(IOperand operand, int[] ids, MatchListMode mode)
		{
			return CreateIn(operand, ids);
		}
		#endregion

		#region CreateNotIn
		/// <summary>
		/// Vytvoří podmínku neexistence hodnoty v poli ID objektů.
		/// </summary>
		/// <param name="operand">Testovaný operand.</param>
		/// <param name="ids">Seznam hodnot. Ověřue se, že hodnota operandu není mezi těmito hodnotami.</param>
		/// <returns>Podmínka testující neexistenci hodnoty v poli ID objektů.</returns>
		public static Condition CreateNotIn(IOperand operand, int[] ids)
		{
			Contract.Requires<ArgumentNullException>(operand != null, nameof(operand));
			Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));

			if (ids.Length == 0)
			{
				return StaticCondition.CreateTrue();
			}
			else
			{
				return new NotCondition(CreateIn(operand, ids));
			}
		}

		/// <summary>
		/// Vytvoří podmínku neexistence hodnoty v poli ID objektů.
		/// </summary>
		/// <param name="operand">Testovaný operand.</param>
		/// <param name="ids">Seznam hodnot. Ověřue se, že hodnota operandu není mezi těmito hodnotami.</param>
		/// <param name="mode">Způsob tvoření dotazu.</param>
		/// <returns>Podmínka testující neexistenci hodnoty v poli ID objektů.</returns>
		[Obsolete("Parameter mode byl zrušen a neuvažuje se.", true)]
		public static Condition CreateNotIn(IOperand operand, int[] ids, MatchListMode mode)
		{
			return CreateNotIn(operand, ids);
		}
		#endregion
	}
}
