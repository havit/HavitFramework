using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje XmlCommentsForDescriptionPropertyConvention do ConventionSetu.
	/// </summary>
	internal class XmlCommentsForDescriptionPropertyConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{			
			// přidíváme se na začátek, protože vestavěná konvence převádí model na readonly
			// pokud se přidáme na konec, je již model readonly a my se dozvíme výjimku NullReferenceException v OnAnnotationSet
			// (bohužel není vyhozena smyslupná výjimka o readonly modelu).
			conventionSet.ModelFinalizedConventions.Insert(0, new XmlCommentsForDescriptionPropertyConvention());
			return conventionSet;
		}
	}
}
