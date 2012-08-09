<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseDropDownListTest.aspx.cs" Inherits="WebApplicationTest.EnterpriseDropDownListTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnterpriseDropDownList 
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="true"
			DataTextField="Symbol"		
			runat="server" 
		/>
		<havit:EnterpriseDropDownList 
			SortDirection="Descending"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="true"
			DataTextField="Symbol"		
			runat="server"
		/>
    </div>
    </form>
</body>
</html>
