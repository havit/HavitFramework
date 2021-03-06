﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="SwitchButtonSample" Src="~/sections/controls/samples/SwitchButtonSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>SwitchButton</h1>
	<p>Touchable Yes/No control with button group appearance</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Appearance</h2>
	<p>Displays touchable Yes/No choice.</p>

	<h2>Properties, Events</h2>
	<ul>
		<li>NoText, YesText (supports $resources pattern)</li>
		<li>NoCssClass, YesCssClass (defaults: "no", "yes")</li>
		<li>Value (True for "Yes", False for "No")</li>
		<li>AutoPostBack</li>
		<li>ValueChanged (event)</li>
	</ul>

	<h2>Styling</h2>
	<p>It is intended to style control by .yes, and .no class in cooperation with .active class and .btn-default or other .btn-* selector. See sample below.</p>

	<h2>Sample</h2>	
	<uc:SwitchButtonSample ID="SwitchButtonSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="SwitchButtonSampleUC" runat="server" />
</asp:Content>
