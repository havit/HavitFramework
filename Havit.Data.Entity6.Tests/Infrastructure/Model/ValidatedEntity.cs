using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Tests.Infrastructure.Model
{
	public class ValidatedEntity
	{
		public int Id { get; set; }

		[Required]
		public string RequiredValue { get; set; }
	}
}
