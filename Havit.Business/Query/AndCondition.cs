using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Havit.Business.Query;

/// <summary>
/// Kompozitní podmínka. Výsledek je pravdivý, jsou-li pravdivé všechny členské podmínky.
/// </summary>
public class AndCondition : CompositeCondition
{
	/// <summary>
	/// Vytvoří kompozitní podmínku. Lze inicializovat sadou členských podmínek.
	/// </summary>		
	public AndCondition(params Condition[] conditions)
		: base("AND", conditions)
	{
	}

	/// <summary>
	/// Vytvoří kompozitní podmínku. Lze inicializovat sadou členských podmínek.
	/// </summary>
	public static AndCondition Create(params Condition[] conditions)
	{
		return new AndCondition(conditions);
	}
}
