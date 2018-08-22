using System;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Entity
{
	public class DbContextActivator
	{
		public DbContext Activate(Type dbContextType)
		{
			return (DbContext)Microsoft.EntityFrameworkCore.Design.DbContextActivator.CreateInstance(dbContextType);
		}

	}
}
