using System;
using System.Collections.Generic;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Model;

public class LoginAccount
{
	public int Id { get; set; }
	
	public IList<Role> Roles { get; set; }

	public DateTime? Deleted { get; set; }
}
