#pragma warning disable 1591
using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

[assembly: WebResource("Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter-IE6.css", "text/css")]

namespace Havit.Web.UI.Adapters.CssAdapters
{
	/// <summary>
	/// MenuAdapter z CSS Friendly Control Adapters. Renderuje Menu pomocí HTML elementů ol, li, CSS stylů a JScriptu.
	/// </summary>
	/// <remarks>
	/// ASP.NET 2.0 CSS Friendly Control Adapters 1.0, http://www.asp.net/CSSAdapters/, This version was last updated on 20 November 2006.
	/// </remarks>
	public class CssMenuAdapter : System.Web.UI.WebControls.Adapters.MenuAdapter
    {
        private WebControlAdapterExtender _extender = null;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (Control != null)) ||
                    ((_extender != null) && (Control != _extender.AdaptedControl)))
                {
                    _extender = new WebControlAdapterExtender(Control);
                }

                System.Diagnostics.Debug.Assert(_extender != null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Extender.AdapterEnabled)
            {
                RegisterScripts();
				RegisterStyles();
            }
        }

        private void RegisterScripts()
        {
            Extender.RegisterScripts();

			Page.ClientScript.RegisterClientScriptResource(typeof(CssMenuAdapter), "Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter.js");
			
			//string folderPath = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path");
			//if (String.IsNullOrEmpty(folderPath))
			//{
			//    folderPath = "~/JavaScript";
			//}
			//string filePath = folderPath.EndsWith("/") ? folderPath + "MenuAdapter.js" : folderPath + "/MenuAdapter.js";
			//Page.ClientScript.RegisterClientScriptInclude(GetType(), GetType().ToString(), Page.ResolveUrl(filePath));
        }

		private void RegisterStyles()
		{
			HttpContext context = HttpContext.Current;

			if (Page.Header != null)
			{
				bool registered = (bool)(context.Items["Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter_registered"] ?? false);

				if (!registered)
				{
					HtmlLink stylesBasic = new HtmlLink();
					stylesBasic.Href = Page.ClientScript.GetWebResourceUrl(typeof(CssMenuAdapter), "Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter.css");
					stylesBasic.Attributes.Add("rel", "stylesheet");
					stylesBasic.Attributes.Add("type", "text/css");
					Page.Header.Controls.Add(stylesBasic);

					if (Browser.Browser.Contains("MSIE") && (Browser.MajorVersion < 7))
					{
						HtmlLink stylesIE6 = new HtmlLink();
						stylesIE6.Href = Page.ClientScript.GetWebResourceUrl(typeof(CssMenuAdapter), "Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter-IE6.css");
						stylesIE6.Attributes.Add("rel", "stylesheet");
						stylesIE6.Attributes.Add("type", "text/css");
						Page.Header.Controls.Add(stylesIE6);
					}

					context.Items["Havit.Web.UI.Adapters.CssAdapters.CssMenuAdapter_registered"] = true;
				}
			}
		}

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderBeginTag(writer, "AspNet-Menu-" + Control.Orientation.ToString());
            }
            else
            {
                base.RenderBeginTag(writer);
            }
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderEndTag(writer);
            }
            else
            {
                base.RenderEndTag(writer);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                writer.Indent++;
                BuildItems(Control.Items, true, writer);
                writer.Indent--;
                writer.WriteLine();
            }
            else
            {
                base.RenderContents(writer);
            }
        }

        private void BuildItems(MenuItemCollection items, bool isRoot, HtmlTextWriter writer)
        {
            if (items.Count > 0)
            {
                writer.WriteLine();

                writer.WriteBeginTag("ul");
                if (isRoot)
                {
                    writer.WriteAttribute("class", "AspNet-Menu");
					writer.WriteAttribute("disappearAfter", ((Menu)Control).DisappearAfter.ToString()); // render desapperAfter attribute
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                               
                foreach (MenuItem item in items)
                {
                    BuildItem(item, writer);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(MenuItem item, HtmlTextWriter writer)
        {
            Menu menu = Control as Menu;
            if ((menu != null) && (item != null) && (writer != null))
            {
                writer.WriteLine();
                writer.WriteBeginTag("li");

                string theClass = (item.ChildItems.Count > 0) ? "AspNet-Menu-WithChildren" : "AspNet-Menu-Leaf";
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    theClass += " " + selectedStatusClass;
                }
                writer.WriteAttribute("class", theClass);

                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                writer.WriteLine();

                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", GetItemClass(menu, item));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent++;
                    writer.WriteLine();

                    MenuItemTemplateContainer container = new MenuItemTemplateContainer(menu.Items.IndexOf(item), item);
                    if ((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null))
                    {						
                        menu.StaticItemTemplate.InstantiateIn(container);
                    }
                    else
                    {
                        menu.DynamicItemTemplate.InstantiateIn(container);
                    }
                    container.DataBind();
                    container.RenderControl(writer);

                    writer.Indent--;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
                else
                {
                    if (IsLink(item))
                    {
                        writer.WriteBeginTag("a");
                        if (!String.IsNullOrEmpty(item.NavigateUrl))
                        {
                            writer.WriteAttribute("href", Page.Server.HtmlEncode(menu.ResolveClientUrl(item.NavigateUrl)));
                        }
                        else
                        {
                            writer.WriteAttribute("href", Page.ClientScript.GetPostBackClientHyperlink(menu, "b" + item.ValuePath.Replace(menu.PathSeparator.ToString(), "\\"), true));
                        }

                        writer.WriteAttribute("class", GetItemClass(menu, item));
                        WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target);

                        if (!String.IsNullOrEmpty(item.ToolTip))
                        {
                            writer.WriteAttribute("title", item.ToolTip);
                        }
                        else if (!String.IsNullOrEmpty(menu.ToolTip))
                        {
                            writer.WriteAttribute("title", menu.ToolTip);
                        }
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("class", GetItemClass(menu, item));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();
                    }

                    if (!String.IsNullOrEmpty(item.ImageUrl))
                    {
                        writer.WriteBeginTag("img");
                        writer.WriteAttribute("src", menu.ResolveClientUrl(item.ImageUrl));
                        writer.WriteAttribute("alt", !String.IsNullOrEmpty(item.ToolTip) ? item.ToolTip : (!String.IsNullOrEmpty(menu.ToolTip) ? menu.ToolTip : item.Text));
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    }

                    writer.Write(item.Text);

                    if (IsLink(item))
                    {
                        writer.Indent--;
                        writer.WriteEndTag("a");
                    }
                    else
                    {
                        writer.Indent--;
                        writer.WriteEndTag("span");
                    }

                }

                if ((item.ChildItems != null) && (item.ChildItems.Count > 0))
                {
                    BuildItems(item.ChildItems, false, writer);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        private bool IsLink(MenuItem item)
        {
            return (item != null) && item.Enabled && ((!String.IsNullOrEmpty(item.NavigateUrl)) || item.Selectable);
        }

        private string GetItemClass(Menu menu, MenuItem item)
        {
            string value = "AspNet-Menu-NonLink";
            if (item != null)
            {
                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    value = "AspNet-Menu-Template";
                }
                else if (IsLink(item))
                {
                    value = "AspNet-Menu-Link";
                }
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    value += " " + selectedStatusClass;
                }
            }
            return value;
        }

        private string GetSelectStatusClass(MenuItem item)
        {
            string value = "";
            if (item.Selected)
            {
                value += " AspNet-Menu-Selected";
            }
            else if (IsChildItemSelected(item))
            {
                value += " AspNet-Menu-ChildSelected";
            }
            else if (IsParentItemSelected(item))
            {
                value += " AspNet-Menu-ParentSelected";
            }
            return value;
        }

        private bool IsChildItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.ChildItems != null))
            {
                bRet = IsChildItemSelected(item.ChildItems);
            }

            return bRet;
        }

        private bool IsChildItemSelected(MenuItemCollection items)
        {
            bool bRet = false;

            if (items != null)
            {
                foreach (MenuItem item in items)
                {
                    if (item.Selected || IsChildItemSelected(item.ChildItems))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsParentItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.Parent != null))
            {
                if (item.Parent.Selected)
                {
                    bRet = true;
                }
                else
                {
                    bRet = IsParentItemSelected(item.Parent);
                }
            }

            return bRet;
        }
    }
}
#pragma warning restore 1591
