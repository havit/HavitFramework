using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class IdWithPoorlyNamedForeignKey
	{
		public int Id { get; set; }

		public IdWithPoorlyNamedForeignKey ForeignKey { get; set; }
		public int ForeignKeyCode { get; set; } // is a foreign key
	}
}
