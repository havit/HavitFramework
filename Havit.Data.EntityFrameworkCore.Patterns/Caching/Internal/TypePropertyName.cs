using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
    internal class TypePropertyName
    {
        public Type Type { get; set; }
        public string PropertyName { get; set; }

        public TypePropertyName(Type type, string propertyName)
        {
            Type = type;
            PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            TypePropertyName second = obj as TypePropertyName;
            return (second != null)
                && (this.Type == second.Type)
                && (this.PropertyName == second.PropertyName);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ PropertyName.GetHashCode();
        }
    }
}
