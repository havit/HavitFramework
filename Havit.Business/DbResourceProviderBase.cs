#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web.Compilation;

namespace Havit.Business;

/// <summary>
/// Base class from local and global resources providers reading values from database (Havit.Business).
/// Supports string values only!
/// </summary>
public abstract class DbResourceProviderBase : IResourceProvider
{
	/// <summary>
	/// Resource class identifier (filename and path for local resources, class name for global resources).
	/// </summary>
	protected string ResourceClass
	{
		get;
		private set;
	}

	/// <summary>
	/// Gets an object to read resource values from a source.
	/// Not supported in this class.
	/// </summary>
	/// <returns>
	/// The <see cref="T:System.Resources.IResourceReader"/> associated with the current resource provider.
	/// </returns>
	public IResourceReader ResourceReader
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DbResourceProviderBase"/> class.
	/// </summary>
	/// <param name="resourceClass">Identifikátor resources třídy (cesta a název souboru pro lokální, název třídy pro globální).</param>
	protected DbResourceProviderBase(string resourceClass)
	{
		Debug.Assert(!String.IsNullOrEmpty(resourceClass));

		this.ResourceClass = resourceClass;
	}

	/// <summary>
	/// Returns a resource object for the key and culture.
	/// </summary>
	/// <param name="resourceKey">The key identifying a particular resource.</param>
	/// <param name="culture">The culture identifying a localized value for the resource.</param>
	/// <returns>
	/// An <see cref="T:System.Object"/> that contains the resource value for the <paramref name="resourceKey"/> and <paramref name="culture"/>.
	/// </returns>
	public object GetObject(string resourceKey, CultureInfo culture)
	{
		Debug.Assert(!String.IsNullOrEmpty(resourceKey)); // contract zajišťuje již interface

		if (IdentityMapScope.Current != null)
		{
			return GetString(resourceKey, culture ?? CultureInfo.CurrentUICulture);
		}
		else
		{
			// Při kompilaci je kontrolováno, zda existují položky resources.
			// Je to mimo životní cyklus stránky, nemáme tedy idenity mapu, kterou pro tyto účely vytvoříme.
			using (new IdentityMapScope())
			{
				return GetString(resourceKey, culture ?? CultureInfo.CurrentUICulture);
			}
		}
	}

	/// <summary>
	/// Template method returning resource string for the resource class, resource key and culture.
	/// Resource class can be determined by ResourceClass property (Value is never null).
	/// </summary>
	/// <param name="resourceKey">The key identifying a particular resource. Value is never null.</param>
	/// <param name="culture">The culture identifying a localized value for the resource. Value is never null.</param>
	/// An <see cref="T:System.String"/> that contains the resource value for the <paramref name="resourceKey"/> and <paramref name="culture"/>.
	protected abstract string GetString(string resourceKey, CultureInfo culture);
}
#endif