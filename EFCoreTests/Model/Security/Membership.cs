using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model.Security
{
	public class Membership
	{
		public LoginAccount LoginAccount { get; set; }
		public int LoginAccountId { get; set; }

		public Role Role { get; set; }
		public int RoleId { get; set; }
	}
}
