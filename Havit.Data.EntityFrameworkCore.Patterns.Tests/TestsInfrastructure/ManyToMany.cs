using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure
{
	public class ManyToMany
	{
		public Language Language { get; set; }
		public int LanguageId { get; set; }

		public ItemWithDeleted ItemWithDeleted { get; set; }
		public int ItemWithDeletedId { get; set; }
	}
}
