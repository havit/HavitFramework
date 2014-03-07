<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ScriptletAjaxTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.ScriptletAjaxTest1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form runat="server">
    <div>
    	
    	<asp:ScriptManager runat="server" />
		
		<asp:UpdatePanel runat="server" UpdateMode="Conditional" >
			<ContentTemplate>
				
				<div>
					<asp:CheckBox ID="TestCheckBox" runat="server" />
					<havit:Scriptlet runat="server">		
						<havit:ControlParameter ControlName="TestCheckBox" runat="server" StartOnChange="true" />
						<havit:ClientScript startonload="true" StartOnAjaxCallback="true" runat="server">
							alert(parameters.TestCheckBox.checked);
						</havit:ClientScript>
					</havit:Scriptlet>		
				</div>
				
				<div>
					<asp:CheckBoxList runat="server" ID="TestCheckBoxList">
						<asp:ListItem Text="1" Value="1" /> 
						<asp:ListItem Text="2" Value="2" /> 
						<asp:ListItem Text="3" Value="3" /> 
					</asp:CheckBoxList>
					<havit:Scriptlet runat="server">		
						<havit:ControlParameter ControlName="TestCheckBoxList" runat="server" StartOnChange="true" />
						<havit:ClientScript runat="server">
							alert(parameters.TestCheckBoxList[0].checked + parameters.TestCheckBoxList[1].checked + parameters.TestCheckBoxList[2].checked);
						</havit:ClientScript>
					</havit:Scriptlet>
				</div>
				
				<div>
					<asp:Repeater ID="TestRepeater" runat="server">
						<ItemTemplate>
							<asp:CheckBox ID="TestCheckBox" runat="server" Text="<%# (string)Container.DataItem %>"/>
							<asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
								<Triggers>
									<asp:AsyncPostBackTrigger runat="server" ControlID="TestCheckBox" />
								</Triggers>
								<ContentTemplate></ContentTemplate>
							</asp:UpdatePanel>
						</ItemTemplate>
					</asp:Repeater>
					<havit:Scriptlet runat="server">		
						<havit:ControlParameter ControlName="TestRepeater" runat="server" StartOnChange="true">
							<havit:ControlParameter ControlName="TestCheckBox" runat="server" StartOnChange="true" />
						</havit:ControlParameter>
						<havit:ClientScript runat="server">
							alert("repeater checkbox changed");
						</havit:ClientScript>
					</havit:Scriptlet>
				</div>

				<div>
					<asp:GridView ID="TestGridView" runat="server">
						<Columns>
							<asp:TemplateField runat="server">
								<ItemTemplate>
									<asp:CheckBox ID="TestCheckBox" runat="server" Text="<%# (string)Container.DataItem %>" />
									<havit:Scriptlet runat="server">		
										<havit:ControlParameter ControlName="TestCheckBox" runat="server" StartOnChange="true" />							
										<havit:ClientScript runat="server">
											alert("gridview checkbox changed");
										</havit:ClientScript>
									</havit:Scriptlet>
								</ItemTemplate>
								
							</asp:TemplateField>
						</Columns>
					</asp:GridView>
				</div>

				<asp:Button runat="server" Text="Callback" ID="CallbackButton" />
				
				<asp:Label runat="server" Text="<%$ Expression: DateTime.Now.ToString() %>" />

			</ContentTemplate>
		</asp:UpdatePanel>

    </div>
    </form>
</body>
</html>
