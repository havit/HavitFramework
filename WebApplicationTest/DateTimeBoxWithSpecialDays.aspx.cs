using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class DateTimeBoxWithSpecialDays : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			SpecialDTB.GetDateTimeBoxCustomization += new Havit.Web.UI.WebControls.DateTimeBox.DateTimeBoxDateCustomizationEventHandler(SpecialDTB_GetDateTimeBoxCustomization);
			Special2DTB.GetDateTimeBoxCustomization += new DateTimeBox.DateTimeBoxDateCustomizationEventHandler(SpecialDTB_GetDateTimeBoxCustomization);

		}

		SpecialDateCustomization specialDateCollection;

		void SpecialDTB_GetDateTimeBoxCustomization(object sender, DateTimeBoxDateCustomizationEventArgs args)
		{
			if (specialDateCollection == null)
			{
				specialDateCollection = new SpecialDateCustomization(GetSpecialDates());
			}

			args.DateCustomization = specialDateCollection; 
		}

		private static List<SpecialDate> GetSpecialDates()
		{
			List<SpecialDate> specialDatesList = new List<SpecialDate>()
			{	new SpecialDate(new DateTime(2012, 1, 1), false, "special"), 
				new SpecialDate(new DateTime(2012, 2, 12), false, "special"),
				new SpecialDate(new DateTime(2012, 4, 9), false, "special"),
				new SpecialDate(new DateTime(2012, 5, 1), false, "special"),
				new SpecialDate(new DateTime(2012, 5, 8), true, String.Empty),
				new SpecialDate(new DateTime(2012, 7, 5), false, "special"),
				new SpecialDate(new DateTime(2012, 7, 6), false, "special"),
				new SpecialDate(new DateTime(2012, 9, 28), false, "special"),
				new SpecialDate(new DateTime(2012, 10, 28), false, "special"),
				new SpecialDate(new DateTime(2012, 11, 17), false, "special"),
				new SpecialDate(new DateTime(2012, 12, 24), false, "special"),
				new SpecialDate(new DateTime(2012, 12, 25), false, "special"),
				new SpecialDate(new DateTime(2012, 12, 26), false, "special"),
				new SpecialDate(new DateTime(2013, 1, 1), false, "special")
			};
			return specialDatesList;
		}
		

	}
}