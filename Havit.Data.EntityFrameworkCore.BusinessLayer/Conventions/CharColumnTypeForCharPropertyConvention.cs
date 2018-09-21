using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre nastavenie typu stĺpca char(1), resp. nchar(1) pre property s typom Char (EF Core štandardne použije nvarchar(1)).
	/// </summary>
    public class CharColumnTypeForCharPropertyConvention : IModelConvention
    {
	    /// <inheritdoc />
	    public void Apply(ModelBuilder modelBuilder)
        {
			foreach (IMutableProperty property in modelBuilder.Model
				.GetApplicationEntityTypes()
				.SelectMany(entityType => entityType.GetProperties())
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