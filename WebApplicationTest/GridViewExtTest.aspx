<%@ Page Language="C#" CodeBehind="GridViewExtTest.aspx.cs" Inherits="WebApplicationTest.GridViewExtTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:GridViewExt ID="TestedGV" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
    </div>
    </form>
</body>
</html>
