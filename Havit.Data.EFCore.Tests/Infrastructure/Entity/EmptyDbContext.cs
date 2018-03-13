using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.EntityFrameworkCore.DbContext;

namespace Havit.Data.EFCore.Tests.Infrastructure.Entity
{
	public class EmptyDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(typeof(EmptyDbContext).FullName);
		}
	}
}
