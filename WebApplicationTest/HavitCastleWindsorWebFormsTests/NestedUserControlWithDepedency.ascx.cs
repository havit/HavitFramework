﻿using System;
using Havit.CastleWindsor.WebForms;
using Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class NestedUserControlWithDepedency : System.Web.UI.UserControl
	{
		#region DisposableComponent
		[Inject]
		public IDisposableComponent DisposableComponent { get; set; }
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			MyLabel.Text = this.DisposableComponent.Hello();
		}
		#endregion
	}
}