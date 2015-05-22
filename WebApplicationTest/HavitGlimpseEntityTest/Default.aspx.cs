﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest.HavitGlimpseEntityTest
{
	public partial class Default : System.Web.UI.Page
	{
		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SyncButton.Click += SyncButton_Click;
			AsyncButton.Click += AsyncButton_Click;
		}
		#endregion

		#region SyncButton_Click
		private void SyncButton_Click(object sender, EventArgs e)
		{
			MyDbContext myDbContext = new MyDbContext();
			//int count = myDbContext.MyClasses.Count();
			List<MyClass> result = myDbContext.MyClasses.OrderBy(item => item.Value).ToList();
		}
		#endregion

		#region AsyncButton_Click
		private void AsyncButton_Click(object sender, EventArgs e)
		{
			MyDbContext myDbContext = new MyDbContext();
			//RegisterAsyncTask(new PageAsyncTask(() => myDbContext.MyClasses.CountAsync()));
			RegisterAsyncTask(new PageAsyncTask(() => myDbContext.MyClasses.OrderBy(item => item.Value).ToListAsync()));
		}
		#endregion

		#region MyDbContext (nested class)
		public class MyDbContext: DbContext 
		{
			public DbSet<MyClass> MyClasses { get; set; }

			public MyDbContext() : base("HavitEntityGlimpse")
			{
				Database.SetInitializer<MyDbContext>(new CreateDatabaseIfNotExists<MyDbContext>());
			}
		}
		#endregion

		#region MyClass (nested class)
		public class MyClass
		{
			public int Id { get; set; }
			
			[MaxLength(Int32.MaxValue)]
			public string Value { get; set; }
		}
		#endregion
	}
}