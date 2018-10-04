using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
    public class Group
    {
		public int Id { get; set; }
		public string Name { get; set; }

		public List<GroupToGroup> Children { get; } = new List<GroupToGroup>();
		public List<GroupToGroup> Parents { get; } = new List<GroupToGroup>();
	}
}
