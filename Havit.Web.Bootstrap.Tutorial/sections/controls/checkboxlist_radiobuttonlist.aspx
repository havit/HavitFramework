<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="CheckBoxListSample" Src="~/sections/controls/samples/CheckBoxListSample.ascx" %>
<%@ Register TagPrefix="uc" TagName="RadioButtonListSample" Src="~/sections/controls/samples/RadioButtonListSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>CheckBoxList &amp; RadioButtonList</h1>
	<p>CheckBoxList and RadioButtonList displayed as button groups</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	
	<h2>Appearance</h2>
	<p>Displays multi-choice or one-of-many choice controls using <a href="http://getbootstrap.com/javascript/#buttons">Bootstrap Button</a>.</p>

	<h2>Properties</h2>
	<ul>
		<li>RepeatLayout (default value overriden to RepeatLayout.Flow and cannot be changed)</li>
		<li>RepeatDirection (default value overriden to RepeatDirection.Horizontal)</li>
		<li>ItemCssClass (default "btn btn-default")</li>
	</ul>

	<h2>ChechBoxList Sample</h2>
	<p>
		<uc:CheckBoxListSample ID="CheckBoxListSampleUC" runat="server" />
	</p>
	<uc:ShowControl ShowControlID="CheckBoxListSampleUC" runat="server" />

	<h2>RadioButtonList Sample</h2>
	<p>
		<uc:RadioButtonListSample ID="RadioButtonListSampleUC" runat="server" />
	</p>
	<uc:ShowControl ShowControlID="RadioButtonListSampleUC" runat="server" />
</asp:Content>
