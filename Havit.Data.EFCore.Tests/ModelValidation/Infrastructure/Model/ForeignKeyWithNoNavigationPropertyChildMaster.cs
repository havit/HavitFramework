using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
    public class ForeignKeyWithNoNavigationPropertyMasterClass
    {
	    public int Id { get; set; }

	    public List<ForeignKeyWithNoNavigationPropertyChildClass> Children { get; set; } 
    }
}
