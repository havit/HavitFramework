using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Havit.EFCoreTests.Model;

public class User
{
	public Guid Id { get; set; }
	public string Username { get; set; } = new Guid().ToString();

	public override string ToString()
	{
		return "User.Id=" + Id;
	}
}
