using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
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
