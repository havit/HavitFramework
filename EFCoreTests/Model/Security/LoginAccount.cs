using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model.Security
{
	public class LoginAccount
	{
		public int Id { get; set; }
		public string Username { get; set; }

		public List<Membership> Memberships { get; } = new List<Membership>();
	}
}
