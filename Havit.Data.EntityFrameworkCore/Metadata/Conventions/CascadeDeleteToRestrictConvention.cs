using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž se zamezí kaskádnímu delete.
	/// </summary>
	public class CascadeDeleteToRestrictConvention : CascadeDeleteConvention
	{
		/// <summary>
		/// Konstructor.
		/// </summary>
		public CascadeDeleteToRestrictConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
		{
		}

		// Podpora pro suppress - není, kdo nechce restrict, nastaví si vlastní hodnotu.

		/// <inheritdoc />
		public override void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			// Systémové tabulky nechceme změnit (byť zde ani nemá cenu řešit)
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			base.ProcessForeignKeyAdded(relationshipBuilder, context);
		}

		/// <inheritdoc />
		public override void ProcessForeignKeyRequirednessChanged(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			// Systémové tabulky nechceme změnit (byť zde ani nemá cenu řešit)
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			base.ProcessForeignKeyRequirednessChanged(relationshipBuilder, context);
		}		

		/// <inheritdoc />
		protected override DeleteBehavior GetTargetDeleteBehavior(IConventionForeignKey foreignKey)
		{
			return DeleteBehavior.Restrict;
		}

	}
}