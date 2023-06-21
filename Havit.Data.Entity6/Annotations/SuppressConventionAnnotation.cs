using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using Havit.Data.Entity.ModelConfiguration.Edm;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Annotations;

internal sealed class SuppressConventionAnnotation : IMergeableAnnotation
{
	/// <summary>
	/// Název konvence použité pro uložení seznamu potlačených konvencí.
	/// </summary>
	internal const string AnnotationName = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:HasSuppressedConvention";

	internal HashSet<Type> SupressedConventions { get; private set; }

	public SuppressConventionAnnotation()
	{
		SupressedConventions = new HashSet<Type>();
	}

	private SuppressConventionAnnotation(IEnumerable<Type> supressedConventions)
	{
		SupressedConventions = new HashSet<Type>(supressedConventions);
	}

	public bool Contains(Type supressedConventions)
	{
		return SupressedConventions.Contains(supressedConventions);
	}

	public void AddSupressedConvention(Type supressedConvention)
	{
		SupressedConventions.Add(supressedConvention);
	}

        public override string ToString()
	{
		return typeof(SuppressConventionAnnotation).Name + ": " + String.Join(", ", SupressedConventions.Select(item => item.FullName).OrderBy(item => item).ToArray());
	}

	public CompatibilityResult IsCompatibleWith(object other)
	{
		if ((other == null) || (other is SuppressConventionAnnotation))
		{
			return new CompatibilityResult(true, "");
		}
		else
		{
			return new CompatibilityResult(false, "Must be type of SuppressConventionAnnotation (or null).");
		}
	}

	public object MergeWith(object other)
	{
		if (Object.ReferenceEquals(this, other) || (other == null))
		{
			return this;
		}

		Contract.Assert<ArgumentException>(other is SuppressConventionAnnotation);

		return new SuppressConventionAnnotation(((SuppressConventionAnnotation)other).SupressedConventions.Union(this.SupressedConventions).ToArray());
	}

	/// <summary>
	/// Indikuje, zda je konvence potlačena.
	/// </summary>
	private static bool IsConventionSuppressed(SuppressConventionAnnotation annotationData, Type conventionType)
	{
		return (annotationData != null) ? annotationData.Contains(conventionType) : false;
	}

	/// <summary>
	/// Indikuje, zda je konvence potlačena.
	/// </summary>
	internal static bool IsConventionSuppressed(StructuralType structuralType, Type conventionType)
	{
		return IsConventionSuppressed((SuppressConventionAnnotation)structuralType.GetAnnotation(SuppressConventionAnnotation.AnnotationName), conventionType);
	}

	/// <summary>
	/// Indikuje, zda je konvence potlačena.
	/// </summary>
	internal static bool IsConventionSuppressed(MetadataItem metadataItem, Type conventionType)
	{
		return IsConventionSuppressed((SuppressConventionAnnotation)metadataItem.GetAnnotation(SuppressConventionAnnotation.AnnotationName), conventionType);
	}
}
