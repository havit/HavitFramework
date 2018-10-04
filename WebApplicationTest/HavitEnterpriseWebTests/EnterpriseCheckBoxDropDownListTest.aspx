<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EnterpriseCheckBoxDropDownListTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitEnterpriseWebTests.EnterpriseDropDownCheckBoxListTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
	<link href="../Content/multiple-select.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>

 		<havit:EnterpriseCheckBoxDropDownList ID="E1"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="false"
			AutoPostBack="true"
			DataTextField="Symbol.Length"		
			runat="server" 
		/>
		<br />		
		<havit:EnterpriseCheckBoxDropDownList ID="E2"
			SortDirection="Ascending"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="true"
			DataTextField="Symbol"
			SortExpression="Symbol"
			AutoSort="true"
			runat="server"
		/>

    </div>
    </form>
</body>
</html>
