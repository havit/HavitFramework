<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckBoxDropDownListTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.DropDownCheckBoxListTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Content/Havit.Web.ClientContent/checkbox-dropdown-list.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server">
        </asp:ScriptManager>

        <div>

            <havit:CheckBoxDropDownList ID="CheckBoxDropDownList1" PlaceHolder="Vyber" ShowSelectAll="true" AllSelectedText="Vybráno vše" AutoPostBack="true" Width="300px" ItemWidth="100px" runat="server">
                <Items>
                    <asp:ListItem Text="Jedna" />
                    <asp:ListItem Text="Dva" />
                    <asp:ListItem Text="Tři" />
                    <asp:ListItem Text="Čtyři" />
                    <asp:ListItem Text="Pět" />
                    <asp:ListItem Text="Šest" />
                </Items>
            </havit:CheckBoxDropDownList>

            <havit:CheckBoxDropDownList ID="CheckBoxDropDownList2" Enabled="false" runat="server">
                <Items>
                    <asp:ListItem Text="Jedna" />
                    <asp:ListItem Text="Dva" />
                    <asp:ListItem Text="Tři" />
                    <asp:ListItem Text="Čtyři" />
                    <asp:ListItem Text="Pět" />
                    <asp:ListItem Text="Šest" />
                </Items>
            </havit:CheckBoxDropDownList>

            <select>
                <option>Test</option>
            </select>

            <asp:Button Text="Postback" runat="server" />
			
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>

            <havit:CheckBoxDropDownList ID="MyListBox" AutoPostBack="True" runat="server">
                <Items>
                    <asp:ListItem Text="Jedna" />
                    <asp:ListItem Text="Dva" />
                    <asp:ListItem Text="Tři" />
                    <asp:ListItem Text="Čtyři" />
                    <asp:ListItem Text="Pět" />
                    <asp:ListItem Text="Šest" />
                </Items>
            </havit:CheckBoxDropDownList>

            <asp:UpdatePanel runat="server">
                <ContentTemplate>
	                
                    <asp:Button Text="Async Postback" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
    </form>
</body>
</html>
