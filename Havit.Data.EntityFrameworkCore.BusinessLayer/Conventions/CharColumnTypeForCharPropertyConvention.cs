using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
    public class CharColumnTypeForCharPropertyConvention : IModelConvention
    {
        public void Apply(ModelBuilder modelBuilder)
        {
            foreach (IMutableProperty property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(char)))
            {
                //Regex.Match(property.Relational().ColumnType, "^(n)?varchar(\\(.*?\\))?$", RegexOptions.IgnoreCase);
                if (property.Relational().ColumnType?.StartsWith("varchar") == true)
                {
                    property.Relational().ColumnType = "char(1)";
                }
                else
                {
                    property.Relational().ColumnType = "nchar(1)";
                }
            }
        }
    }
}