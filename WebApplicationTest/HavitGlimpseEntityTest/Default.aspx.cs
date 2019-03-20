using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitGlimpseEntityTest
{
	public partial class Default : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SyncButton.Click += SyncButton_Click;
			AsyncButton.Click += AsyncButton_Click;
		}

		private void SyncButton_Click(object sender, EventArgs e)
		{
			MyDbContext myDbContext = new MyDbContext();
			//int count = myDbContext.MyClasses.Count();
			List<MyClass> result = myDbContext.MyClasses.OrderBy(item => item.Value).ToList();

			myDbContext.MyClasses.Add(new MyClass { Value = "hello" });
			myDbContext.SaveChanges();
		}

		private void AsyncButton_Click(object sender, EventArgs e)
		{		
			//RegisterAsyncTask(new PageAsyncTask(() => myDbContext.MyClasses.CountAsync()));
			//RegisterAsyncTask(new PageAsyncTask(() =>
			//{
			//	MyDbContext myDbContext = new MyDbContext();
			//	myDbContext.MyClasses.OrderBy(item => item.Value).ToListAsync();
			//}));

			RegisterAsyncTask(new PageAsyncTask(() =>
			{
				MyDbContext myDbContext2 = new MyDbContext();
				myDbContext2.MyClasses.Add(new MyClass { Value = "hello async" });
				return myDbContext2.SaveChangesAsync();
			}));
		}

		public class MyDbContext : DbContext
		{
			public DbSet<MyClass> MyClasses { get; set; }

			public MyDbContext() : base("HavitEntityGlimpse")
			{
				Database.SetInitializer<MyDbContext>(new CreateDatabaseIfNotExists<MyDbContext>());
			}
		}

		public class MyClass
		{
			public int Id { get; set; }
			
			[MaxLength(Int32.MaxValue)]
			public string Value { get; set; }
		}
	}
}