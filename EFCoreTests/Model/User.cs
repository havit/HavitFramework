using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Havit.EFCoreTests.Model
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }

		public override string ToString()
		{
			return "User.Id=" + Id;
		}
	}
}
