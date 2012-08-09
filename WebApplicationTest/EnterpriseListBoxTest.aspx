<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EnterpriseListBoxTest.aspx.cs" Inherits="WebApplicationTest.EnterpriseListBoxTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnterpriseListBox ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" SelectionMode="Multiple" AutoDataBind="true" DataTextField="Symbol" DataSortField="Symbol.Length" SortDirection="Descending" runat="server" />
    </div>
    </form>
</body>
</html>
