using Havit.Data.EntityFrameworkCore.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model.Security
{
	[Cache]
	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
