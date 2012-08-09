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
	}
}
