using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne
{
	public class ClassA
	{
		public int Id { get; set; }
		public ClassB ClassB { get; set; } // no foreign key - "backreference"
	}
}
