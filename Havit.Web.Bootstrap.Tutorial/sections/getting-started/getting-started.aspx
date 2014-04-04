<%@ Page Language="C#" MasterPageFile="~/Templates/Main.Master" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>Getting started</h1>
	<p>Implementing Havit.Web.Bootstrap in project</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Installation</h2>
	<p>Install NuGet package Havit.Bootstrap.Web from <a href="http://wiki.havit.cz/NuGet.ashx">HAVIT NuGet feed</a> into ASP.NET Web Application project.</p>

	<h3>NuGet Package Dependencies</h3>
	<ul>
		<li><a href="http://www.nuget.org/packages/jQuery/">jQuery</a></li>
		<li><a href="http://www.nuget.org/packages/Twitter.Bootstrap.Less/">Twitter.Bootstrap.Less</a> (install LESS files into project)</li>
	</ul>

	<h2>Content</h2>
	<h3>Files</h3>
	<pre><code>content/
└── havit.web.bootstrap/
</code></pre>
	<p>LESS files for styling Havit.Web.Bootstrap controls.</p>
	<h3>Controls registration</h3>
	<p>Controls in Havit.Web.Bootstrap assembly are registered with <strong>bc</strong> prefix.</p>
	<pre><code>&lt;add tagPrefix="bc" namespace="Havit.Web.Bootstrap.UI.WebControls" assembly="Havit.Web.Bootstrap"  /&gt;</code></pre>	

	<h3>Tag mapping</h3>
	<a name="tagmapping" />
	<p>Validator are remapped to the controls of Havit.Web.Bootstrap assembly.</p>
	<pre><code>&lt;tagMapping&gt;
	&lt;add tagType="System.Web.UI.WebControls.CompareValidator" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.CompareValidator" /&gt;
	&lt;add tagType="System.Web.UI.WebControls.CustomValidator" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.CustomValidator" /&gt;
	&lt;add tagType="System.Web.UI.WebControls.RangeValidator" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.RangeValidator" /&gt;
	&lt;add tagType="System.Web.UI.WebControls.RegularExpressionValidator" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.RegularExpressionValidator" /&gt;
	&lt;add tagType="System.Web.UI.WebControls.RequiredFieldValidator" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.RequiredFieldValidator" /&gt;
	&lt;add tagType="System.Web.UI.WebControls.ValidationSummary" mappedTagType="Havit.Web.Bootstrap.UI.WebControls.ValidationSummary" /&gt;
&lt;/tagMapping&gt;</code></pre>
</asp:Content>
