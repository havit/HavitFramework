﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model
{
	public class Person
	{
		public int Id { get; set; }

		public Person Boss { get; set; }
		public int? BossId { get; set; }

		public List<Person> Subordinates { get; } = new List<Person>();
	}
}
