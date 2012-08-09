<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseCheckBoxListTest.aspx.cs" Inherits="WebApplicationTest.EnterpriseCheckBoxListTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

 		<havit:EnterpriseCheckBoxList ID="E1"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="false"
			AutoPostBack="true"
			DataTextField="Symbol.Length"		
			runat="server" 
		/>
		<br />		
		<havit:EnterpriseCheckBoxList ID="E2"
			SortDirection="Ascending"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="true"
			DataTextField="Symbol"
			DataSortField="Symbol"
			AutoSort="true"
			runat="server"
		/>

    </div>
    </form>
</body>
</html>
