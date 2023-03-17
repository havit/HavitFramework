using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class IdWithNoForeignKeyButAllowed
	{
		public int Id { get; set; }
		public int MyId { get; set; } // is not a foreign key, but is allowed in configuration (db context)
	}
}
