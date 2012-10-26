using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using Havit.Reflection;

namespace Havit.Web.UI.HtmlControls
{
	/// <summary>
	/// Rozšíření .NET controlu HtmlForm.
	/// </summary>
	public class HtmlFormExt : System.Web.UI.HtmlControls.HtmlForm
	{
		#region Data members
		/// <summary>
		/// Vrátí nebo nastaví cílové URL formuláře. Atribut Action formuláře.
		/// Pokud není explicitně nastaveno, vrací automaticky Microsoft implementaci (včetně QueryStringu).
		/// </summary>
		/// <remarks>Nedělá se ResolveUrl.</remarks>
		[
			Description("Vrátí nebo nastaví cílové URL formuláře. Atribut Action formuláře."),
			Category("Behavior"),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual string Action
		{
			get
			{
				object action = ViewState["Action"];
				if (action != null)
				{
					return (string)action;
				}
				else
				{
					return this.GetBaseActionAttribute();
				}
			}
		}
		#endregion

		#region RenderAttributes
		/// <summary>
		/// Overriden. Zajišťuje vlastní renderování atributu Action
		/// </summary>
		protected override void RenderAttributes(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteAttribute("name", this.Name);
			this.Attributes.Remove("name");

			writer.WriteAttribute("method", this.Method);
			this.Attributes.Remove("method");

			writer.WriteAttribute("action", this.Action, true);
			this.Attributes.Remove("action");

			string submitEvent = this.Page_ClientOnSubmitEvent;
			if ((submitEvent != null) && (submitEvent.Length > 0))
			{
				if (this.Attributes["onsubmit"] != null) 
				{
					submitEvent = submitEvent + this.Attributes["onsubmit"];
					this.Attributes.Remove("onsubmit");
				}
				writer.WriteAttribute("language", "javascript");
				writer.WriteAttribute("onsubmit", submitEvent);
			}

			if (this.ID == null)
			{
				writer.WriteAttribute("id", this.ClientID);
			}
			
			// nelze volat base.RenderAttributes(), takže
			// voláno z HtmlContainerControl
			this.ViewState.Remove("innerhtml");

			// voláno v HtmlControl
			this.Attributes.Render(writer);
		}
		#endregion

		#region GetBaseActionAttribute, Page_ClientOnSubmitEvent
		/// <summary>
		/// Pomocí reflexe vrátí původní private base.GetActionAttribute()
		/// </summary>
		/// <returns>Microsoft implementace action atributu formuláře</returns>
		private string GetBaseActionAttribute() 
		{
			Type formType = typeof(System.Web.UI.HtmlControls.HtmlForm);
			MethodInfo actionMethod = formType.GetMethod("GetActionAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
			object result = actionMethod.Invoke(this,null);
			return (string)result;
		}

		/// <summary>
		/// Pomocí reflexe vrátí internal ClientOnSubmitEvent vlastnost Page
		/// </summary>
		private string Page_ClientOnSubmitEvent 
		{
			get 
			{
				return (string)Reflector.GetPropertyValue(this.Page, typeof(System.Web.UI.Page), "ClientOnSubmitEvent");
			}
		}
		#endregion
	}
}
