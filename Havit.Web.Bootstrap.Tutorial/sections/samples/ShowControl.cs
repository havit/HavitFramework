using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.FriendlyUrls;

namespace Havit.Web.Bootstrap.Tutorial.Section.Samples
{
	public class ShowControl : HyperLink
	{
		#region Title
		/// <summary>
		/// ...
		/// </summary>
		public string Title
		{
			get
			{
				return (string)(ViewState["Title"] ?? String.Empty);
			}
			set
			{
				ViewState["Title"] = value;
			}
		}
		#endregion

		#region ShowControlID
		public string ShowControlID
		{
			get
			{
				return (string)(ViewState["ShowControlID"] ?? String.Empty);
			}
			set
			{
				ViewState["ShowControlID"] = value;
			}
		}
		#endregion

		#region Constructor
		public ShowControl()
		{
			Text = "Show code";
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			UserControl uc = (UserControl)NamingContainer.FindControl(ShowControlID);
			NavigateUrl = String.Format("~/sections/samples/show?title={0}&control={1}",
				HttpUtility.UrlEncode(Title),
				HttpUtility.UrlEncode(uc.AppRelativeVirtualPath.Replace("~/", "").Replace(".ascx", "")));
		}
		#endregion
	}
}