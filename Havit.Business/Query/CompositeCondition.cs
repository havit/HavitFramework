using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Havit.Business.Query
{
	/// <summary>
	/// Pøedek kompozitních podmínek.
	/// </summary>
	[Serializable]
	public abstract class CompositeCondition : Condition
	{
		private string operatorBetweenOperands = null;

		#region Conditions
		/// <summary>
		/// Podmínky v kompozitu.
		/// </summary>
		public ConditionList Conditions
		{
			get { return conditions; }
		}
		private ConditionList conditions = new ConditionList();
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci.
		/// </summary>
		protected CompositeCondition(string operatorBetweenOperands, params Condition[] conditions)
		{
			this.operatorBetweenOperands = operatorBetweenOperands;
			for (int index = 0; index < conditions.Length; index++)
			{
				this.Conditions.Add(conditions[index]);
			}
		}
		#endregion

		#region ICondition Members
		/// <summary>
		/// Poskládá èlenské podmínky. Mezi podmínkami (operandy) je operátor zadaný v konstruktoru.
		/// </summary>
		public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			Debug.Assert(whereBuilder != null);

			if (Conditions.Count == 0)
			{
				return;
			}

			if (Conditions.Count == 1)
			{			
				Conditions[0].GetWhereStatement(command, whereBuilder);
				return;
			}

			whereBuilder.Append("(");
			for (int i = 0; i < Conditions.Count; i++)
			{
				if (i > 0)
				{
					whereBuilder.AppendFormat(" {0} ", operatorBetweenOperands);
				}
				Conditions[i].GetWhereStatement(command, whereBuilder);
			}
			whereBuilder.Append(")");
		}
		#endregion
	}
}
