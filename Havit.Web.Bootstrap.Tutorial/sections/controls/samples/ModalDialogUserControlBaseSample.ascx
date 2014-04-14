<%@ Register TagPrefix="uc" TagName="ModalDialogSample" Src="~/sections/controls/samples/ModalDialogSample.ascx" %>

<asp:Button Text="Show Dialog" OnClick="ShowDialog_Click" runat="server"/>
<uc:ModalDialogSample ID="ModalDialogSampleUC" runat="server" />

<script language="c#" runat="server">
	protected void ShowDialog_Click(object sender, EventArgs args)
	{
		ModalDialogSampleUC.Show();
	}
</script>

