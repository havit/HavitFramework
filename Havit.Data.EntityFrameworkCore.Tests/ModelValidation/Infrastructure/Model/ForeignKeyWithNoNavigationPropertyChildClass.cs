namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
    public class ForeignKeyWithNoNavigationPropertyChildClass
    {
	    public int Id { get; set; }

		/// <summary>
		/// Foreign key, viz ForeignKeyWithNoNavigationPropertyMasterClass
		/// </summary>
	    public int MasterId { get; set; } 
    }
}
