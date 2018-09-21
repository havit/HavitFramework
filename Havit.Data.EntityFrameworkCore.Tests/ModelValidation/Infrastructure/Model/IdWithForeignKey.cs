using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class IdWithForeignKey
	{
		public int Id { get; set; }

		public IdWithForeignKey ForeignKey { get; set; }
		public int ForeignKeyId { get; set; } // is a foreign key
	}
}
