using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Havit.Business.Query;

/// <summary>
/// Seznam podmínek, který nemůže obsahovat prázdnou podmínku.
/// </summary>
public class ConditionList : Collection<Condition>
{
	/// <summary>
	/// Předefinování metody pro vkládání podmínek. Není možné vložit null (hodnota null je ignorována a není přidána do kolekce.
	/// </summary>
	protected override void InsertItem(int index, Condition item)
	{
		if (item != null)
		{
			base.InsertItem(index, item);
		}
	}
}
