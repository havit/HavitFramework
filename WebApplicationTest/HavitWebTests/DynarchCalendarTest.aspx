<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynarchCalendarTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.DynarchCalendarTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:TextBox ID="MyTB" runat="server" />
		<asp:Button ID="CalendarBt" Text="Calendar" runat="server" />
		<havit:DynarchCalendar Button="CalendarBt" InputField="MyTB" ShowsTime="True" Visible="true" runat="server" />
    </div>
    </form>
</body>
</html>
