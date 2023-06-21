using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Přepis třídy Microsoft .NET Frameworku. Potřeba pro GridViewCommandField.
/// </summary>
[SupportsEventValidation]
internal sealed class DataControlButtonExt : Button
{
	// Fields
	private readonly IPostBackContainer _container;

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

	public override bool UseSubmitBehavior
	{
		get
		{
			return false;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	// Methods

	internal DataControlButtonExt(IPostBackContainer container)
	{
		this._container = container;
	}

	protected sealed override PostBackOptions GetPostBackOptions()
	{
		if (this._container != null)
		{
			PostBackOptions postBackOptions = this._container.GetPostBackOptions(this);
			if (this.Page != null)
			{
				postBackOptions.ClientSubmit = true;
			}
			return postBackOptions;
		}
		return base.GetPostBackOptions();
	}
}
