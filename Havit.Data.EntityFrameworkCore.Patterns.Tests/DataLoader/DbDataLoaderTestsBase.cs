using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	public abstract class DbDataLoaderTestsBase
	{
		protected void SeedOneToManyTestData(DataLoaderTestDbContext dbContext = null, bool deleted = false)
		{
			if (dbContext == null)
			{
				dbContext = new DataLoaderTestDbContext();
			}

			dbContext.Database.DropCreate();

			for (int i = 0; i < 5; i++)
			{
				Master master = new Master();
				if (deleted)
				{
					master.Deleted = DateTime.Now;
				}

				for (int j = 0; j < 5; j++)
				{
					Child child = new Child();
					child.Parent = master;
					if (deleted)
					{
						child.Deleted = DateTime.Now;
					}

					dbContext.Master.Add(master);
					dbContext.Child.Add(child);
				}
			}

			dbContext.SaveChanges();
		}

		protected void SeedManyToManyTestData(bool deleted = false)
		{
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();

			for (int i = 0; i < 5; i++)
			{
				Role role = new Role();
				if (deleted)
				{
					role.Deleted = DateTime.Now;
				}

				LoginAccount loginAccount = new LoginAccount();
                loginAccount.Memberships = new List<Membership> { new Membership { LoginAccount = loginAccount, Role = role } };
				if (deleted)
				{
					loginAccount.Deleted = DateTime.Now;
				}

				dbContext.LoginAccount.Add(loginAccount);
				dbContext.Role.Add(role);
			}

			dbContext.SaveChanges();
		}
	}
}
