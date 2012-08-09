using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Havit.Business.Query;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using System.Collections.Generic;

namespace WebApplicationTest
{
    public partial class JirkkaTest : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerOpen1Button.Click += new EventHandler(ServerOpen1Button_Click);
            ServerClose1Button.Click += new EventHandler(ServerClose1Button_Click);

            ServerOpen2Button.Click += new EventHandler(ServerOpen2Button_Click);
            ServerClose2Button.Click += new EventHandler(ServerClose2Button_Click);

            EnlargeButton.Click += new EventHandler(EnlargeButton_Click);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DataBind();
        }

        private void ServerOpen1Button_Click(object sender, EventArgs e)
        {
            MyBasicModalDialog.Show();
        }

        private void ServerClose1Button_Click(object sender, EventArgs e)
        {
            MyBasicModalDialog.Hide();
        }

        private void ServerOpen2Button_Click(object sender, EventArgs e)
        {
            MyAjaxModalDialog.Show();
        }

        private void ServerClose2Button_Click(object sender, EventArgs e)
        {
            MyAjaxModalDialog.Hide();
        }

        private void EnlargeButton_Click(object sender, EventArgs e)
        {
            MyAjaxModalDialog.Width = new Unit(MyAjaxModalDialog.Width.Value + 10, UnitType.Pixel);
            MyAjaxModalDialog.Height = new Unit(MyAjaxModalDialog.Height.Value + 10, UnitType.Pixel);
        }


        [WebMethod()]
        public static string GetSuggestions(string keyword, bool usePaging, int pageIndex, int pageSize)
        {
            QueryParams qp = new QueryParams();
            qp.Conditions.Add(TextCondition.CreateWildcards(Subjekt.Properties.Nazev, keyword));
            qp.TopRecords = pageSize;

            SubjektCollection subjekty = Subjekt.GetList(qp);

            List<AutoSuggestMenuItem> menuItems = new List<AutoSuggestMenuItem>(subjekty.Count);
            foreach (Subjekt subjekt in subjekty)
            {
                menuItems.Add(new AutoSuggestMenuItem(subjekt.Nazev, subjekt.ID.ToString()));
            }

            return AutoSuggestMenu.ConvertMenuItemsToJSON(menuItems, menuItems.Count);
        }

		protected override PageStatePersister PageStatePersister
		{
			get
			{
				if (_currentPageStatePersister == null)
				{
					_currentPageStatePersister = new FilePageStatePersister(this, System.IO.Path.GetTempPath());
				}
				return _currentPageStatePersister;
			}
		}
		PageStatePersister _currentPageStatePersister;
    }

	public class FilePageStatePersister: PageStatePersister
	{
		private const string FilenameWithUserPattern = "viewstate.{1}\\{0}";
		private const string FilenameWithoutUserPattern = "viewstate.anonymous\\{0}";

		private string _viewstatePath;

		public FilePageStatePersister(Page page, string viewstatePath): base(page)
		{
			_viewstatePath = viewstatePath;
		}

		public override void Load()
		{
			string state = System.IO.File.ReadAllText(GetFilename(Page.Request.Form["__VIEWSTATE_STORAGE"]));
			Pair pair = (Pair)StateFormatter.Deserialize(state);
			ViewState = pair.First;
			ControlState = pair.Second;
		}

		public override void Save()
		{
			string guid = Guid.NewGuid().ToString();
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(GetFilename(guid)));
			using (System.IO.StreamWriter writer = System.IO.File.CreateText(GetFilename(guid)))
			{
				writer.Write(StateFormatter.Serialize(new Pair(ViewState, base.ControlState)));
			}
			this.Page.ClientScript.RegisterHiddenField("__VIEWSTATE_STORAGE", guid);
		}

		public static void DeleteAllUserFiles(string viewstatePath, string username)
		{
			string[] files = System.IO.Directory.GetFiles(viewstatePath, String.Format(FilenameWithUserPattern, "*", username.GetHashCode() % 1000000), System.IO.SearchOption.AllDirectories);
			foreach (string file in files)
			{
				System.IO.File.Delete(file);
			}
		}
		public static void DeleteOldFiles(string viewstatePath)
		{
			// to je blbě
			string[] files = System.IO.Directory.GetFiles(viewstatePath, "*.viewstate", System.IO.SearchOption.AllDirectories);
			DateTime yesterday = DateTime.Today.AddDays(-1);
			foreach (string file in files.Where(file => System.IO.File.GetCreationTime(file) < yesterday))
			{
				System.IO.File.Delete(file);
			}
		}

		private string GetFilename(string code)
		{
			return System.IO.Path.Combine(_viewstatePath, string.Format(Page.User.Identity.IsAuthenticated ? FilenameWithUserPattern : FilenameWithoutUserPattern, code, Page.User.Identity.Name.GetHashCode() % 1000000));
		}
	}
}
