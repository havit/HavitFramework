using System.Web.UI;

namespace Havit.Web.UI;

/// <summary>
/// Pomocné metody pro práci s Controlem.
/// </summary>
internal static class ControlExtensions
{
	/// <summary>
	/// Vrátí všechny vnořené controls, které splňují podmínku predicate. Prohledává do hloubky, avšak do INamingContainer jde pouze, pokud je to určeno parametrem traverseNestedNamingContainers.
	/// </summary>
	internal static IEnumerable<Control> FindControls(this Control control, Predicate<Control> predicate, bool traverseNestedNamingContainers)
	{
		if (!control.HasControls())
		{
			yield break;
		}

		foreach (Control nestedControl in control.Controls)
		{
			if (predicate(nestedControl))
			{
				yield return nestedControl;
			}

			if (traverseNestedNamingContainers || (!(nestedControl is INamingContainer)))
			{
				foreach (Control foundControl in FindControls(nestedControl, predicate, traverseNestedNamingContainers))
				{
					yield return foundControl;
				}
			}
		}
	}
}
