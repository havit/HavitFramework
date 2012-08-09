using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	/// <summary>
	/// Kompozitn� podm�nka. V�sledek je pravdiv�, je-li pravdiv� alespo� jedna �lensk� podm�nka.
	/// </summary>
	[Serializable]
	public class OrCondition: CompositeCondition
	{
		/// <summary>
		/// Vytvo�� kompozitn� podm�nku. Lze inicializovat sadou �lensk�ch podm�nek.
		/// </summary>		
		public OrCondition(params ICondition[] conditions) : base("or", conditions)
		{
		}
	}
}
