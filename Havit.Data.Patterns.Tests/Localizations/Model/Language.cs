using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Tests.Localizations.Model
{
	public class Language : ILanguage
	{
		public int Id { get; set; }

		public string Culture { get; set; }

		public string UiCulture { get; set; }
	}
}
