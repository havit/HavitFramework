using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Pøepis tøídy Microsoft .NET Frameworku. Potøeba pro GridViewCommandField.
	/// </summary>
	[SupportsEventValidation]
	internal sealed class DataControlButtonExt : Button
	{
		// Fields
		private IPostBackContainer _container;

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
	}

}
