<%@ Page Language="C#" MasterPageFile="~/Templates/Main.Master" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>LESS</h1>
	<p>General guidelines</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Recommended structure</h2>
	<pre><code>content/
├── bootstrap/bootstrap.less
│   ├── bootstrap.less
│   └── (+ other less files from Twitter.Bootstrap.Less NuGet package)
├── havit.web.bootstrap/
│   ├── havit.web.bootstrap.less
│   └── (+ other less files from Havit.Web.Bootstrap NuGet package)
└── site.less
</code></pre>
	
	<h2>site.less</h2>
	<p>Main project less file which generated css should be included in master page.</p>
	<p>Generated css file must be included in project (and really is by defalt) because build copies for deployment only project files.</p>

	<pre><code>@import "bootstrap/bootstrap.less";
@import "toastr.less";
@import "havit.web.bootstrap/havit.web.bootstrap.less";

...
</code></pre>
	<h2>Other resources</h2>
	<ul>
		<li>
			<a href="http://ruby.bvision.com/blog/please-stop-embedding-bootstrap-classes-in-your-html">Please stop embedding Bootstrap classes in your HTML!</a>			
		</li>
	</ul>
</asp:Content>
