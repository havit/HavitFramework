using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class ParentEntity
{
	public int Id { get; set; }

	public List<ChildEntity> Children { get; } = new List<ChildEntity>();
}
