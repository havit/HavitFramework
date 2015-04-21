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
│   ├── havit-web-bootstrap.less
│   └── (+ other less files from Havit.Web.Bootstrap NuGet package)
└── site.less
</code></pre>
	
	<h2>site.less</h2>	
	<p>Main project less file which generated css should be included in master page.</p>	
	<p>Generated css file must be included in project (and really is by defalt) because build copies for deployment only project files.</p>

	<pre><code>@import "bootstrap/bootstrap.less";
@import "toastr.less";
@import "havit.web.bootstrap/havit-web-bootstrap.less";

...
</code></pre>
		
	<div class="alert-danger">
		<blockquote>
			Never modify bootstrap or havit-web-bootstrap LESS files (or LESS files from other dependency package) otherwise your changes will be overwritten in next NuGet package update. Use LESS variables or css class inheritance in site.less (or other less file which you have in your hands).
		</blockquote>
	</div>

	<h2>Other resources</h2>
	<ul>
		<li>
			<a href="http://ruby.bvision.com/blog/please-stop-embedding-bootstrap-classes-in-your-html">Please stop embedding Bootstrap classes in your HTML!</a><br /><br />
		</li>
		<li>
			<a href="http://www.helloerik.com/bootstrap-3-grid-introduction">Bootstrap 3 Grid Introduction</a>
		</li>
		<li>
			<a href="http://www.helloerik.com/the-subtle-magic-behind-why-the-bootstrap-3-grid-works">The Subtle Magic Behind Why the Bootstrap 3 Grid Works</a>
		</li>
		<li>
			<a href="http://www.helloerik.com/bootstrap-3-less-workflow-tutorial">Bootstrap 3 Less Workflow Tutorial</a>
		</li>
	</ul>
</asp:Content>
