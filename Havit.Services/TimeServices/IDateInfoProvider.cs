using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TimeServices
{
	/// <summary>
	/// Interface for service providing DateInfo data.
	/// Supports WorkDaysCalculator.
	/// </summary>
	public interface IDateInfoProvider
	{
		/// <summary>
		/// Returns IDateInfo for the specified day. If there is none, returns null.
		/// Whould be implemented as O(1) whenever possible (Dictionary).
		/// </summary>
		IDateInfo GetDateInfo(DateTime date);
	}
}
