using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.EFCoreTests.Model
{
	[Owned]
	public class Address
	{
		public string Street { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
	}
}
