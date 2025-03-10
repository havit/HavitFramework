using System.ComponentModel.DataAnnotations;
using Havit.Model.Localizations;

namespace Havit.Data.EntityFrameworkCore.TestSolution.Model.Common;

public class Language : ILanguage
{
	public int Id { get; set; }

	[MaxLength(50)]
	public string Name { get; set; }

	public string Culture { get; set; }

	public string UiCulture { get; set; }
}