﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Infrastructure
{
	internal class ValidatingDbContext : DbContext
	{
		static ValidatingDbContext()
		{
			System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<ValidatingDbContext>());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.RegisterEntityType(typeof(ValidatedEntity));
		}
	}
}
