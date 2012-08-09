using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Havit.Business.Query
{
	/// <summary>
	/// Seznam podmínek, který nemùže obsahovat prázdnou podmínku.
	/// </summary>
	public class ConditionList: Collection<Condition>
	{
		/// <summary>
		/// Pøedefinování metody pro vkládání podmínek. Není možné vložit null (hodnota null je ignorována a není pøidána do kolekce.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, Condition item)
		{
			if (item != null)
			{
				base.InsertItem(index, item);
			}
		}		
	}
}
