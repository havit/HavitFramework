using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Havit.Business.Query
{
	/// <summary>
	/// Pøedek kompozitních podmínek.
	/// </summary>
	[Serializable]
	public abstract class CompositeCondition : List<ICondition>, ICondition
	{
		string operatorBetweenOperands = null;

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci.
		/// </summary>
		public CompositeCondition(string operatorBetweenOperands, params ICondition[] conditions)
		{
			this.operatorBetweenOperands = operatorBetweenOperands;
			this.AddRange(conditions);
		}
		#endregion

		#region ICondition Members
		/// <summary>
		/// Poskládá èlenské podmínky. Mezi podmínkami (operandy) je operátor zadaný v konstruktoru.
		/// </summary>
		public virtual void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder)
		{
			if (Count == 0)
				return;

			if (Count == 1)
			{			
				this[0].GetWhereStatement(command, whereBuilder);
				return;
			}

			whereBuilder.Append("(");
			for (int i = 0; i < Count; i++)
			{
				if (i > 0)
					whereBuilder.AppendFormat(" {0} ", operatorBetweenOperands);
				this[i].GetWhereStatement(command, whereBuilder);
			}
			whereBuilder.Append(")");
		}
		#endregion
	}
}
