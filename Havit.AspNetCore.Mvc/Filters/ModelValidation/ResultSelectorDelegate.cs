using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Havit.AspNetCore.Mvc.Filters.ModelValidation;

/// <summary>
/// Vrací odpověď v případě nevalidního ModelState.
/// </summary>
public delegate object ResultSelectorDelegate(int statusCode, ModelStateDictionary modelStateDictionary);