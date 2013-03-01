using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Repository IPersisterControlExtenderu.
	/// </summary>
	public class PersisterControlExtenderRepository : List<IPersisterControlExtender>
	{
		#region Default
		/// <summary>
		/// Výchozí repository. Obsahuje "standardní" IPersisterControlExtendery.
		/// </summary>
		public static PersisterControlExtenderRepository Default
		{
			get
			{
				if (_default == null)
				{
					lock (_defaultLock)
					{
						if (_default == null)
						{
							_default = new PersisterControlExtenderRepository();
							_default.Add(new CheckBoxPersisterControlExtender());  // RadioButton dědí z CheckBoxu, proto je v důsledku použito i pro zpracování hodnoty RadioButtonu.
							_default.Add(new TextBoxPersisterControlExtender());
							_default.Add(new DropDownListPersisterControlExtender());
							_default.Add(new NumericBoxPersisterControlExtender());
							_default.Add(new DateTimeBoxPersisterControlExtender());
							_default.Add(new GridViewExtPersisterControlExtender());
							_default.Add(new RadioButtonListPersisterControlExtender());
							_default.Add(new EnumDropDownListPersisterControlExtender());
							_default.Add(new CheckBoxListPersisterControlExtender());
							_default.Add(new ListBoxPersisterControlExtender());
						}
					}
				}
				return _default;
			}
		}
		private static PersisterControlExtenderRepository _default;
		private static object _defaultLock = new object();
		#endregion

		#region FindExtender
		/// <summary>
		/// Vyhledá pro předaný control IPersisterControlExtender, který bude control zpracovávat.
		/// Pokud není žádný vhodný extender nalezen, je vrácena hodnota null.
		/// Bere v ohledu prioritu, s jakou jsou ControlExtendery vhodné pro daný control.
		/// </summary>
		/// <param name="control">Control ke kterému se hledá IPersisterControlExtender.</param>
		/// <returns>Nalezený IPersisterControlExtender.</returns>
		public IPersisterControlExtender FindExtender(Control control)
		{
			int bestExtenderPriority = Int32.MinValue;
			IPersisterControlExtender bestExtender = null;

			this.ForEach(delegate(IPersisterControlExtender currentExtender)
			{
				int? currentPriority = currentExtender.GetPriority(control);
				if (currentPriority != null && currentPriority >= bestExtenderPriority)
				{
					bestExtenderPriority = currentPriority.Value;
					bestExtender = currentExtender;
				}
			});

			return bestExtender;
		}
		#endregion

		#region GetExtenderValuesTypes
		/// <summary>
		/// Vrací seznam návratových typů extenderů, které se nachází v repository.
		/// </summary>
		internal Type[] GetExtenderValuesTypes()
		{
			List<Type> valueTypes = new List<Type>();

			foreach (IPersisterControlExtender extender in this)
			{
				Type type = extender.GetValueType();
				if (!valueTypes.Contains(type))
				{
					valueTypes.Add(type);
				}
			}

			return valueTypes.ToArray();
		}
		#endregion
		
	}
}
