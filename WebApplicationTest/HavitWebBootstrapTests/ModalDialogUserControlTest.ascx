<%@ Control Language="C#" CodeBehind="ModalDialogUserControlTest.ascx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ModalDialogUserControlTest" %>

<bc:ModalDialog ID="MainModalDialog" HeaderText="My cool dialog!" runat="server">
	<ContentTemplate>	
		Hello! This is my first cool dialog.
		<br/>
		<asp:CheckBox ID="MyCheckBox" runat="server" />
		<havit:DateTimeBox runat="server"/>
		
	<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean in orci quis lectus varius dapibus. Praesent arcu. Mauris ullamcorper pretium mauris. Nullam ut ligula. Aliquam orci mi, condimentum luctus, consectetuer ut, hendrerit sagittis, risus. Morbi dolor. Nulla blandit consequat lectus. Sed rutrum faucibus risus. Donec placerat. Aliquam erat volutpat. Curabitur nec lorem. Morbi volutpat euismod augue. Ut sollicitudin vestibulum elit. Fusce ut dui at eros pharetra ullamcorper. Etiam leo neque, feugiat sed, consectetuer in, aliquam id, nisl. In vitae nisl ac magna mollis adipiscing.
	Maecenas semper, elit eget egestas laoreet, lacus lorem mattis libero, non vehicula odio metus sit amet pede. Praesent non orci vel justo mollis blandit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Phasellus tempor, sapien sit amet condimentum pellentesque, est nisi pretium sapien, nec scelerisque massa nisl sed arcu. Integer dictum commodo tortor. Etiam egestas, libero id iaculis rutrum, enim tellus vehicula justo, et sodales felis turpis non mauris. Cras vehicula auctor mauris. Vivamus ipsum elit, fringilla et, venenatis ut, sagittis in, risus. Suspendisse condimentum aliquet urna. Suspendisse sed pede ornare urna cursus commodo. Sed quis nisi ac sapien molestie convallis. Aenean vel tellus. Pellentesque lobortis massa vel velit.
	Ut semper. Vivamus sagittis mattis purus. Duis sem pede, aliquam nec, facilisis ut, ornare id, ipsum. Vivamus tincidunt nunc sed eros. Proin pede. Nullam imperdiet augue vel elit. Ut fermentum mi non eros vestibulum accumsan. Donec laoreet, metus at pellentesque luctus, ante lacus sollicitudin purus, ac varius lorem diam in urna. Ut suscipit sapien in felis. Vestibulum leo. Pellentesque est nunc, sollicitudin ac, imperdiet eget, convallis luctus, lectus. Ut in libero. Proin ante sem, volutpat sit amet, tincidunt sed, imperdiet sed, urna.
	Suspendisse fringilla diam mattis risus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam consectetuer, ante eget sollicitudin pulvinar, ante tellus malesuada mi, sit amet mollis risus mauris et eros. Nullam risus erat, congue id, pulvinar ut, lacinia ac, odio. Duis a risus vitae lacus blandit ornare. Vivamus vitae sapien. Nulla facilisi. Nam eros augue, vulputate ac, placerat eu, sagittis id, lectus. Aliquam erat volutpat. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Ut enim. Phasellus quis augue rutrum orci tincidunt euismod. Cras metus velit, porta sit amet, eleifend ac, congue quis, orci. Sed eu massa vehicula sapien porttitor fringilla. Integer non purus sit amet tortor viverra imperdiet. Vivamus ipsum. Pellentesque nunc risus, pellentesque id, lacinia in, accumsan ac, orci. Fusce convallis nisl vel ante. Curabitur id tellus in massa sagittis gravida. In vulputate pretium sapien.

	</ContentTemplate>
	<FooterTemplate>
		<asp:Button ID="CloseButton" Text="Close" runat="server" />
		<asp:Button Text="Refresh" runat="server" />
	</FooterTemplate>	
</bc:ModalDialog>