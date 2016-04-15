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

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Language);
		}

		protected bool Equals(Language other)
		{
			return Id == other?.Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}

		public static bool operator ==(Language left, Language right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Language left, Language right)
		{
			return !Equals(left, right);
		}
	}
}
