﻿using System.Web.UI.WebControls;
using System.Web.UI;
using System.Drawing;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Přepis třídy Microsoft .NET Frameworku. Potřeba pro GridViewCommandField.
/// </summary>
[SupportsEventValidation]
internal class DataControlLinkButtonExt : LinkButton
{
	private string _callbackArgument;
	private readonly IPostBackContainer _container;
	private bool _enableCallback;

	public override bool CausesValidation
	{
		get
		{
			if (this._container != null)
			{
				return false;
			}
			return base.CausesValidation;
		}
		set
		{
			if (this._container != null)
			{
				throw new NotSupportedException("CannotSetValidationOnDataControlButtons");
			}
			base.CausesValidation = value;
		}
	}

	internal DataControlLinkButtonExt(IPostBackContainer container)
	{
		this._container = container;
	}

	internal void EnableCallback(string argument)
	{
		this._enableCallback = true;
		this._callbackArgument = argument;
	}

	protected override PostBackOptions GetPostBackOptions()
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
		this.SetForeColor();
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

	protected virtual void SetForeColor()
	{
		if (ControlStyle.ForeColor == Color.Empty) // (!base.ControlStyle.IsSet(4))
		{
			Control parent = this;
			for (int i = 0; i < 3; i++)
			{
				parent = parent.Parent;
				Color foreColor = ((WebControl)parent).ForeColor;
				if (foreColor != Color.Empty)
				{
					this.ForeColor = foreColor;
					return;
				}
			}
		}
	}
}
