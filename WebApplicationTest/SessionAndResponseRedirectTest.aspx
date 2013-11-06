<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionAndResponseRedirectTest.aspx.cs" Inherits="WebApplicationTest.SessionAndResponseRedirectTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
		<havit:MessengerControl runat="server"/>
		<div>
			<asp:Label ID="CounterLabel" runat="server"/>
		</div>
		<div>
			<asp:Button ID="TrueButton" text="true" runat="server" />
			<asp:Button ID="FalseButton" Text="false" runat="server" />
		</div>
    </form>
</body>
</html>
