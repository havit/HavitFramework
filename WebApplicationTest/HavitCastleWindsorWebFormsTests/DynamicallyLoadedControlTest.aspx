<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicallyLoadedControlTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests.DynamicallyLoadedControlTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Dynamically Loaded Control Test (Havit.CastleWindsor.WebForms)</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<asp:PlaceHolder ID="MyPH" runat="server" />
		</div>
	</form>
</body>
</html>
