using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.Metadata
{
    public static class LanguageEntityExtensions
    {
        public const string LanguageEntityName = "Language";

        public static bool IsLanguageEntity(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return entityType.Name == LanguageEntityName;
        }

        public static IMutableProperty GetUICultureProperty(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return entityType.FindProperty("UiCulture");
        }
    }
}