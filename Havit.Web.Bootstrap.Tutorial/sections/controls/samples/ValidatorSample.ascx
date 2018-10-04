<asp:ValidationSummary ValidationGroup="Sample" runat="server" />
<p>
	<asp:TextBox ID="SampleTextBox" runat="server"/>
	<bc:RequiredFieldValidator ControlToValidate="SampleTextBox" ErrorMessage="Enter value to the textbox." ValidationGroup="Sample" runat="server"/>
</p>
<p>
	<asp:Button Text="Validate!" ValidationGroup="Sample" runat="server"/>
</p>