using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
	/// V�choz� implementace <see cref="IControlExtenderRepository">IControlExtenderRepository</see>.
    /// </summary>
    public class ControlExtenderRepository : List<IControlExtender>, IControlExtenderRepository
	{
		#region Default (static)
		/// <summary>
		/// V�choz� seznam control extender�.
		/// </summary>
		public static ControlExtenderRepository Default
		{
			get
			{
				lock (typeof(ControlExtenderRepository))
				{
					if (_default == null)
					{
						_default = new ControlExtenderRepository();
						_default.Add(new SimpleControlExtender(typeof(TextBox), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(CheckBox), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(RadioButton), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(Button), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(DropDownList), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(Havit.Web.UI.WebControls.NumericBox), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(Havit.Web.UI.WebControls.DateTimeBox), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(WebControl), 10, null));
						_default.Add(new ListControlExtender(typeof(RadioButtonList), 100));
						_default.Add(new ListControlExtender(typeof(CheckBoxList), 100));
						_default.Add(new RepeaterControlExtender(100));
						_default.Add(new GridViewControlExtender(100));
					}
				}
				return _default;
			}
		}
		private static ControlExtenderRepository _default;
		#endregion

		#region FindControlExtender
		/// <summary>
		/// Nalezne pro control extender, kter� bude control zpracov�vat.
		/// Pokud nen� ��dn� vhodn� extender nalezen, je vyhozena v�jimka HttpException.
		/// </summary>
		/// <param name="control">Control ke zpracov�n�.</param>
		/// <returns>Nalezen� control extender.</returns>
		public IControlExtender FindControlExtender(Control control)
		{
			int bestExtenderPriority = Int32.MinValue;
			IControlExtender bestExtender = null;

			this.ForEach(delegate(IControlExtender currentExtender)
			{
				int? currentPriority = currentExtender.GetPriority(control);
				if (currentPriority != null && currentPriority >= bestExtenderPriority)
				{
					bestExtenderPriority = currentPriority.Value;
					bestExtender = currentExtender;
				}
			});

			if (bestExtender == null)
			{
				throw new HttpException(String.Format("Nebyl nalezen ControlExtender pro control {0}.", control.ID));
			}

			return bestExtender;
		}
		#endregion
    }
}
