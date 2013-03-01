using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Přepis třídy Microsoft .NET Frameworku. Potřeba pro GridViewCommandField.
	/// </summary>
	[SupportsEventValidation]
	internal sealed class DataControlButtonExt : Button
	{
		// Fields
		#region Private fields
		private IPostBackContainer _container;
		#endregion

		// Properties

		#region CausesValidation
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
		#endregion

		#region UseSubmitBehavior
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
		#endregion

		// Methods

		#region DataControlButtonExt
		internal DataControlButtonExt(IPostBackContainer container)
		{
			this._container = container;
		}
		#endregion

		#region GetPostBackOptions
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
		#endregion

	}

}
