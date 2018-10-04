using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Havit.AspNetCore.Mvc.Filters.ModelValidation
{
	/// <summary>
	/// Určí stavový kódu odpovědi.		
	/// </summary>
	public delegate int StatusCodeSelectorDelegate(ModelStateDictionary modelStateDictionary);
}