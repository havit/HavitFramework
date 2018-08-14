using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EFCore.Tests.Infrastructure.Model
{
	[NotMapped]
	public class NotMappedClass
	{
		public int Id { get; set; }
		
		[MaxLength(100)]
		public string Name { get; set; }
	}
}
