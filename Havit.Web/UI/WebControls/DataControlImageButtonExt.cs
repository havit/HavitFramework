using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Pøepis tøídy Microsoft .NET Frameworku. Potøeba pro GridViewCommandField.
	/// </summary>
	[SupportsEventValidation]
	internal sealed class DataControlImageButtonExt : ImageButton
	{
		// Fields
		private string _callbackArgument;
		private IPostBackContainer _container;
		private bool _enableCallback;

		// Methods
		internal DataControlImageButtonExt(IPostBackContainer container)
		{
			this._container = container;
		}

		internal void EnableCallback(string argument)
		{
			this._enableCallback = true;
			this._callbackArgument = argument;
		}

		protected sealed override PostBackOptions GetPostBackOptions()
		{
			if (getPostBackOptionsDisabled)
			{
				// brutální oprava dvojího vyvolání event (dva requesty na server) na image buttonech
				// viz AddAttributesToRender
				return null;
			}

			if (this._container != null)
			{
				return this._container.GetPostBackOptions(this);
			}
			return base.GetPostBackOptions();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.SetCallbackProperties();
			base.Render(writer);
		}

		private void SetCallbackProperties()
		{
			if (this._enableCallback)
			{
				ICallbackContainer container = this._container as ICallbackContainer;
				if (container != null)
				{
					string callbackScript = container.GetCallbackScript(this, this._callbackArgument);
					if (!string.IsNullOrEmpty(callbackScript))
					{
						this.OnClientClick = callbackScript;
					}
				}
			}
		}

		// Properties
		public override bool CausesValidation
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotSupportedException("CannotSetValidationOnDataControlButtons");
			}
		}

		private bool getPostBackOptionsDisabled = false;
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			// brutální oprava dvojího vyvolání event (dva requesty na server) na image buttonech
			// <input type="image" src="image.gif" onclick="javascript:__doPostBack('GridView1','Delete$0')" />
			// První request vznikne z type="image",
			// druhý request vznikne z __doPostBack.
			if (base.IsEnabled)
			{
				string onClick = Attributes["onclick"];
				string onClientClick = this.OnClientClick;
				Attributes.Remove("onclick");
				this.OnClientClick = "";

				PostBackOptions postBackOptions = this.GetPostBackOptions();
				Page.ClientScript.RegisterForEventValidation(postBackOptions);
				string postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, false);
				string result = EnsureStartWithJavascript(MergeScript(MergeScript(MergeScript(onClientClick, onClick), postBackEventReference), "return false;"));

				getPostBackOptionsDisabled = true;
				base.AddAttributesToRender(writer);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, result);
				getPostBackOptionsDisabled = false;

			}
			else
			{
				base.AddAttributesToRender(writer);
			}

		}

		internal static string MergeScript(string firstScript, string secondScript)
		{
			if (String.IsNullOrEmpty(secondScript))
			{
				return firstScript ?? String.Empty;
			}

			if (String.IsNullOrEmpty(firstScript))
			{
				return secondScript ?? String.Empty;
			}

			return EnsureEndWithSemiColon(firstScript) + EnsureEndWithSemiColon(secondScript);
		}

		internal static string EnsureEndWithSemiColon(string value)
		{
			if (value != null)
			{
				int length = value.Length;
				if ((length > 0) && (value[length - 1] != ';'))
				{
					return (value + ";");
				}
			}
			return value;
		}

		internal static string EnsureStartWithJavascript(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return value;
			}

			if (value.TrimStart(new char[0]).StartsWith("javascript:"))
			{
				return value;
			}
			return "javascript:" + value;
		}
	}
}
