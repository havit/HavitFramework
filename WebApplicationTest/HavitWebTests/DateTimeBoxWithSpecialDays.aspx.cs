using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class DateTimeBoxWithSpecialDays : System.Web.UI.Page
{
	private SpecialDateCustomization specialDateCollection;

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		//DateTimeBox.GetDateTimeBoxCustomizationDefault += new DateTimeBox.DateTimeBoxDateCustomizationEventHandler(SpecialDTB_GetDateTimeBoxCustomization);
		//SpecialDTB.GetDateTimeBoxCustomization += new Havit.Web.UI.WebControls.DateTimeBox.DateTimeBoxDateCustomizationEventHandler(SpecialDTB_GetDateTimeBoxCustomization);			
		Special2DTB.GetDateTimeBoxCustomization += new DateTimeBox.DateTimeBoxDateCustomizationEventHandler(SpecialDTB_GetDateTimeBoxCustomization2);
	}

	//void SpecialDTB_GetDateTimeBoxCustomization(object sender, DateTimeBoxDateCustomizationEventArgs args)
	//{
	//    if (specialDateCollectionDefault == null)
	//    {
	//        specialDateCollectionDefault = new SpecialDateCustomization(GetSpecialDates());
	//    }

	//    args.DateCustomization = specialDateCollectionDefault; 
	//}		

	private void SpecialDTB_GetDateTimeBoxCustomization2(object sender, DateTimeBoxDateCustomizationEventArgs args)
	{
		if (specialDateCollection == null)
		{
			specialDateCollection = new SpecialDateCustomization(GetSpecialDates2());
		}

		args.DateCustomization = specialDateCollection;
	}

	private static List<SpecialDate> GetSpecialDates()
	{
		List<SpecialDate> specialDatesList = new List<SpecialDate>()
		{
			new SpecialDate(new DateTime(2012, 1, 1), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 2, 12), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 4, 9), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 5, 1), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 5, 8), disabled: true, String.Empty),
			new SpecialDate(new DateTime(2012, 7, 5), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 7, 6), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 9, 28), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 10, 28), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 11, 17), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 12, 24), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 12, 25), disabled: false, "special"),
			new SpecialDate(new DateTime(2012, 12, 26), disabled: false, "special"),
			new SpecialDate(new DateTime(2013, 1, 1), disabled: false, "special")
		};
		return specialDatesList;
	}

	private static List<SpecialDate> GetSpecialDates2()
	{
		List<SpecialDate> specialDatesList = new List<SpecialDate>()
		{
			new SpecialDate(new DateTime(2013, 8, 8), disabled: true, String.Empty),
			new SpecialDate(new DateTime(2013, 8, 9), disabled: false, "special"),
			new SpecialDate(new DateTime(2013, 8, 10), disabled: false, "special")
		};
		return specialDatesList;
	}
}