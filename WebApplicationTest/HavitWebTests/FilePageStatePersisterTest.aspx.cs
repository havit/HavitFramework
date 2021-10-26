using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;
using static Havit.Web.UI.FilePageStatePersister;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class FilePageStatePersisterTest : System.Web.UI.Page
	{
		private const string ViewStateStorage = @"\\topol\Workspace\002.HFW\ViewState";
		
		private readonly FilePageStatePersister pageStatePersister;

		public FilePageStatePersisterTest()
		{
			pageStatePersister = new Web.UI.FilePageStatePersister(this, ViewStateStorage, FileStoragePageStatePersisterSerializationStrategy.LosFormatter);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DeleteOldAnonymousFilesButton.Click += DeleteOldAnonymousFilesButton_Click;
		}

		protected override PageStatePersister PageStatePersister => pageStatePersister;

		private void DeleteOldAnonymousFilesButton_Click(object sender, EventArgs e)
		{
			PerUserNamingStrategy.DeleteOldAnonymousFiles(ViewStateStorage, TimeSpan.FromMinutes(1));
		}
	}
}