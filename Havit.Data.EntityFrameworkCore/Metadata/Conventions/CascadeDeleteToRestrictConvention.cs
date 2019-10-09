using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž zamezí kaskádnímu delete.
	/// </summary>
	public class CascadeDeleteToRestrictConvention : CascadeDeleteConvention
	{
		// TODO EF Core 3.0: Podpora pro suppress! Je to vůbec implementovatelné? Spíš ne.
		// TODO EF Core 3.0: Comment, Proč je to potomek CascadeDeleteConvention, proč je doplněn after a nikoliv replacenut (kvůli suppressu)

		/// <summary>
		/// Konstructor.
		/// </summary>
		public CascadeDeleteToRestrictConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
		{
		}

		/// <inheritdoc />
		protected override DeleteBehavior GetTargetDeleteBehavior(IConventionForeignKey foreignKey)
		{
			return DeleteBehavior.Restrict;
		}

	}
}