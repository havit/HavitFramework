using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Localizations.Internal
{
	/// <summary>
	/// Úložiště párování culture na jazyk.
	/// </summary>
	public class LanguageByCultureStorage : ILanguageByCultureStorage
	{
		/// <summary>
		/// Úložiště párování cultura jazyka.
		/// </summary>
		public Dictionary<string, int> Value { get; set; }
	}
}
