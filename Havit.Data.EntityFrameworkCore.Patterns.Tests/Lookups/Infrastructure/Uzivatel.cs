using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure
{
	public class Uzivatel
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public DateTime? Deleted { get; set; }
	}
}
