using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls;

internal interface IRadioButtonListCheckBoxList
{
	ListItemCollection Items { get; }
	RepeatLayout RepeatLayout { get; }
	RepeatDirection RepeatDirection { get; }
	int RepeatColumns { get; }
	string CssClass { get; set; }
	string ItemCssClass { get; set; }
	bool HtmlEncode { get; set; }
	AttributeCollection Attributes { get; }
	void AddAttributesToRender(HtmlTextWriter writer);
	void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer);
}
