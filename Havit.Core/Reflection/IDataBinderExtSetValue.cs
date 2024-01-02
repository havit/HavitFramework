using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Reflection;

/// <summary>
/// Interface that marks the type whose DataBinderExt.SetValue should work in a non-standard way.
/// Intended for setting the value of business object collections, where we do not want to invoke their setter, but want to perform cleaning and filling of the collection.
/// </summary>
public interface IDataBinderExtSetValue
{
	/// <summary>
	/// Sets the value of the object from the parameter.
	/// </summary>
	void SetValue(object value);
}
