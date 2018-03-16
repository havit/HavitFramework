using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Metadata.Internal
{
	/// <summary>
	/// Extension metody k <see cref="InternalModelBuilder" />.
	/// </summary>
    public static class InternalModelBuilderExtensions
    {
		/// <summary>
		/// Vrací <see cref="ConventionSet"/> použitý v daném <see cref="InternalModelBuilder"/>.
		/// Používá reflexi a vytahuje hodnotu z privátního fieldu. Je jisté, že v dalších verzích budeme způsob získávání instance měnit.
		/// </summary>
	    public static ConventionSet GetConventionSet(this InternalModelBuilder internalModelBuilder)
	    {
		    ConventionDispatcher conventionDispatcher = internalModelBuilder.Metadata.ConventionDispatcher;
		    object immediateConventionScope = conventionDispatcher.GetType().GetField("_immediateConventionScope", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(conventionDispatcher);
		    ConventionSet conventionSet = (ConventionSet)immediateConventionScope.GetType().GetField("_conventionSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(immediateConventionScope);
		    return conventionSet;
	    }
    }
}
