using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
	/// Výchozí implementace <see cref="IControlExtenderRepository">IControlExtenderRepository</see>.
    /// </summary>
    public class ControlExtenderRepository : List<IControlExtender>, IControlExtenderRepository
	{
		#region Default (static)
		/// <summary>
		/// Výchozí seznam control extenderů.
		/// </summary>
		public static ControlExtenderRepository Default
		{
			get
			{
				lock (_defaultLock)
				{
					if (_default == null)
					{
						_default = new ControlExtenderRepository();
						_default.Add(new SimpleControlExtender(typeof(TextBox), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(CheckBox), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(RadioButton), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(Button), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(LinkButton), 100, new string[] { "onclick" }));
						_default.Add(new SimpleControlExtender(typeof(DropDownList), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(FileUpload), 100, new string[] { "onchange" }));
						_default.Add(new SimpleControlExtender(typeof(HiddenField), 100, null));
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
		private static object _defaultLock = new object();
		#endregion

		#region FindControlExtender
		/// <summary>
		/// Nalezne pro control extender, který bude control zpracovávat.
		/// Pokud není žádný vhodný extender nalezen, je vyhozena výjimka HttpException.
		/// </summary>
		/// <param name="control">Control ke zpracování.</param>
		/// <returns>Nalezený control extender.</returns>
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
