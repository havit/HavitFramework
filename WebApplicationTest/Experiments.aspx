<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Experiments.aspx.cs" Inherits="WebApplicationTest.Experiments" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<script>
			function TestF(a, b, c)
			{
				alert("a=" + a + " b=" + b + " c=" + c);
			}
			
			var i = 0;
			function inc()
			{
				i = i + 1;
			}
			function go()
			{
				var c = 444;			
				var fce = function(a, b)
				{
					TestF.apply(null, [inc(), b, fce]);
				}
				c = 555;
				inc();
				fce('11', 222);
				c = 666;
			}
		</script>
		<input type="button" onclick="go();"
    </div>
    </form>
</body>
</html>
