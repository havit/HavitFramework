<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoSuggestMenuControl.ascx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.AutoSuggestMenuControl" %>
<asp:TextBox ID="SubjektTB" Width="150" runat="server" Style="border: 1px solid black;" />
<havit:AutoSuggestMenu
	ID="SubjektASM"
	TargetControlID="SubjektTB"
	ServicePath="~/HavitWebTests/AutoSuggestMenuService.asmx"
	KeyPressDelay="300"
	UsePaging="true"
	PageSize="10"
	MinSuggestChars="2"
	OnGetSuggestions="Havit.WebApplicationTest.HavitWebTests.AutoSuggestMenuService.GetSuggestions"
	Mode="ClearTextOnNoSelection"
	MessageOnClearText="Hodnota stornována, nedošlo ke spárování s číselníkovou hodnotou."
	AutoPostBack="true"
	UsePageMethods="false"
	runat="server" />
