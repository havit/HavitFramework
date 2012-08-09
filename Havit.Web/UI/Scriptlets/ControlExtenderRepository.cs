using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Výchozí implementace IControlExtenderRepository.
    /// </summary>
    public class ControlExtenderRepository : List<IControlExtender>, IControlExtenderRepository
	{
		private static ControlExtenderRepository _default;

        /// <summary>
        /// Výchozí seznam control extenderù.
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
                        _default.Add(new SimpleControlExtender(typeof(WebControl), 10, null));
                        _default.Add(new RepeaterControlExtender(100));
                        _default.Add(new ListControlExtender(typeof(RadioButtonList), 100));
                        _default.Add(new ListControlExtender(typeof(CheckBoxList), 100));
                    }
				}
				return _default;
			}
		}

        /// <summary>
        /// Nalezne pro control extender, který bude control zpracovávat.
        /// Pokud není žádný vhodný extender nalezen, je vyhozena výjimka.
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
                throw new ArgumentException(String.Format("Nebyl nalezen ControlExtender pro control {0}.", control.ID));

            return bestExtender;
        }

    }
}
