﻿using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model
{
	public class LoginAccount
	{
		public int Id { get; set; }

        public List<Membership> Memberships { get; set; }

		public DateTime? Deleted { get; set; }
	}
}