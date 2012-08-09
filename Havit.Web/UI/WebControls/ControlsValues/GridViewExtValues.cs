using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Collections;
using System.Xml.Serialization;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Goran.WebBase.UI.WebControls.ControlsValues
{	
	public class GridViewExtValue
	{
		public int PageIndex
		{
			get;
			set;
		}

		public SortItemCollection SortItems
		{
			get;
			set;
		}
	}
}
