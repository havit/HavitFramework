using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	[Owned]
	public class OwnedType
	{
		public string Name { get; set; }
	}
}
