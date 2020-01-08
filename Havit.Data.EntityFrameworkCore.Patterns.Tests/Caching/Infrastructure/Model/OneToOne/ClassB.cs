using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne
{
	public class ClassB
	{
		public int Id { get; set; }
		
		public ClassA ClassA { get; set; }
		public int ClassAId { get; set; }
	}
}
