using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class ChildEntity
{
	public int Id { get; set; }

	public ParentEntity Parent { get; set; }
}
