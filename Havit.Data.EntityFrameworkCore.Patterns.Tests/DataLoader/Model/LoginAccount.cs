using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model
{
	public class LoginAccount
	{
		public int Id { get; set; }
		
		public List<Membership> Roles { get; set;  }

		public DateTime? Deleted { get; set; }
	}
}
