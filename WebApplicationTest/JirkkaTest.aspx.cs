using System;
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
    }
}
