using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EFCore.Tests.Infrastructure.Model
{
	public class ModelClass
	{
		public int Id { get; set; }
		
		[MaxLength(100)]
		public string Name { get; set; }
	}
}
