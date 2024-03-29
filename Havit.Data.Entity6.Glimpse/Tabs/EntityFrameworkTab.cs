﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glimpse.Core.Extensions;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;

using Havit.Data.Entity.Glimpse.Model;

namespace Havit.Data.Entity.Glimpse.Tabs;

/// <summary>
/// Entity Framework (EF) tab.
/// </summary>
public class EntityFrameworkTab : ITab, ITabLayout, ITabSetup/*, IKey*/
{
	/// <summary>
	/// Gets when the <see cref="ITab.GetData" /> method should run.
	/// </summary>
	/// <value>The execute on.</value>
	public RuntimeEvent ExecuteOn
	{
		get
		{
			return RuntimeEvent.EndRequest;
		}
	}

	/// <summary>
	/// Gets the type of the request context that the Tab relies on. If
	/// returns null, the tab can be used in any context.
	/// </summary>
	/// <value>The type of the request context.</value>
	public Type RequestContextType
	{
		get
		{
			return null;
		}
	}

	/// <summary>
	/// Gets the name that will show in the tab.
	/// </summary>
	/// <value>The name.</value>
	public string Name
	{
		get
		{
			return "EF";
		}
	}

	/// <summary>
	/// Gets the data that should be shown in the UI.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>Object that will be shown.</returns>
	public object GetData(ITabContext context)
	{			
		return new EntityFrameworkTabData
		{
			Commands = context.GetMessages<DbCommandMessage>().ToList() 
		};
		// objekt je dále naformátován/zobrazen pomocí SerializactionConverteru
	}

	/// <summary>
	/// Layout.
	/// </summary>
	public object GetLayout()
	{
		if (_layout == null)
		{
			_layout = TabLayout.Create().Row(r =>
			{
				r.Cell(0).WidthInPixels(115); /* Operation */
				r.Cell(1).AsCode(CodeType.Sql).WidthInPercent(55); /* CommandText */
				r.Cell(2); /* Parameters */
				r.Cell(3).WidthInPixels(25); /* Tx */
				r.Cell(4).WidthInPixels(70).AlignRight(); /* Result */
				r.Cell(5).WidthInPixels(70).Suffix(" ms").AlignRight().Class("mono"); /* Duration */
			}).Build();
		}
		return _layout;
	}
	private object _layout;

	/// <summary>
	/// Setups the targeted tab using the specified context.
	/// </summary>
	/// <param name="context">The context which should be used.</param>
	public void Setup(ITabSetupContext context)
	{
		context.PersistMessages<DbCommandMessage>();
	}
}
