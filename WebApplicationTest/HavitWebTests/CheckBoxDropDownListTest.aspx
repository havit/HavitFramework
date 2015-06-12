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

					<havit:CheckBoxDropDownList ID="MyListBox" AutoPostBack="True" LeaveOpenInAutoPostBack="True" width="10em" runat="server">
						<Items>
							<asp:ListItem Text="Jedna" />
							<asp:ListItem Text="Dva" />
							<asp:ListItem Text="Tři" />
							<asp:ListItem Text="Čtyři" />
							<asp:ListItem Text="Pět" />
							<asp:ListItem Text="Šest" />
							<asp:ListItem Text="Sedm" />
							<asp:ListItem Text="Osm" />
							<asp:ListItem Text="Devět" />
							<asp:ListItem Text="Deset" />
							<asp:ListItem Text="11" />
							<asp:ListItem Text="12" />
							<asp:ListItem Text="13" />
							<asp:ListItem Text="14" />
							<asp:ListItem Text="15" />
							<asp:ListItem Text="16" />
							<asp:ListItem Text="17" />
						</Items>
					</havit:CheckBoxDropDownList>
			
			<asp:UpdatePanel runat="server">
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="MyListBox" />
				</Triggers>
                <ContentTemplate>

	                
                    <asp:Button Text="Async Postback" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
    </form>
</body>
</html>
