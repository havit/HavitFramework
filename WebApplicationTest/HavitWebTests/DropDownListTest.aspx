<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DropDownListTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.DropDownListTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
	    <havit:DropDownListExt runat="server">
		    <Items>
			    <asp:ListItem Value="1" Text="Jedna" OptionGroup="Liché"></asp:ListItem>
			    <asp:ListItem Value="3" Text="Tři" OptionGroup="Liché"></asp:ListItem>
			    <asp:ListItem Value="5" Text="Pět" OptionGroup="Liché"></asp:ListItem>
			    <asp:ListItem Value="7" Text="Sedm" OptionGroup="Liché"></asp:ListItem>
			    <asp:ListItem Value="2" Text="Dva" OptionGroup="Sudé"></asp:ListItem>
			    <asp:ListItem Value="4" Text="Čtyři" OptionGroup="Sudé"></asp:ListItem>
			    <asp:ListItem Value="6" Text="Šest" OptionGroup="Sudé"></asp:ListItem>
			    <asp:ListItem Value="8" Text="Osm" OptionGroup="Sudé"></asp:ListItem>
		    </Items>
	    </havit:DropDownListExt>

	    <havit:DropDownListExt ID="SudeLicheDDL" runat="server">
		    <Items>
			    <asp:ListItem Value="1" Text="1" />
			    <asp:ListItem Value="2" Text="2" />
			    <asp:ListItem Value="3" Text="3" />
			    <asp:ListItem Value="4" Text="4" />
			    <asp:ListItem Value="5" Text="5" />
			    <asp:ListItem Value="6" Text="6" />
		    </Items>
	    </havit:DropDownListExt>

	    <havit:DropDownListExt runat="server">
		    <Items>
			    <asp:ListItem Value="nic" Text="nic"></asp:ListItem>
			    <asp:ListItem Value="1" Text="1" OptionGroup="Liche"></asp:ListItem>
			    <asp:ListItem Value="3" Text="3" OptionGroup="Liche"></asp:ListItem>
			    <asp:ListItem Value="nic" Text="nic"></asp:ListItem>
			    <asp:ListItem Value="2" Text="2" OptionGroup="Sude"></asp:ListItem>
			    <asp:ListItem Value="4" Text="4" OptionGroup="Sude"></asp:ListItem>
			    <asp:ListItem Value="nic" Text="nic"></asp:ListItem>
		    </Items>
	    </havit:DropDownListExt>


		<havit:GridViewExt ID="TestGridView" runat="server">
			<Columns>
				<havit:TemplateFieldExt>
					<ItemTemplate>
						<havit:EnterpriseDropDownList
							ID="RoleDDL"
							ItemObjectInfo="<%$ Expression: Havit.BusinessLayerTest.Role.ObjectInfo %>"
							SelectedObject="<%# Havit.BusinessLayerTest.Role.GetObject(-1) %>"
							DataTextField="Symbol"
							DataOptionGroupField="ID"
							AutoDataBind="false"
							AutoSort="false"
							runat="server" />
					</ItemTemplate>
				</havit:TemplateFieldExt>
			</Columns>
		</havit:GridViewExt>
		<asp:Button ID="PostbackButton" Text="Postback" runat="server" />
    </div>
    </form>
</body>
</html>
