using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class IdWithNoForeignKey
	{
		public int Id { get; set; }
		public int MyId { get; set; } // is not a foreign key
	}
}
