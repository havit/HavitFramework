using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Web.Compilation;
using System.Web.UI;

namespace Havit.Web.Compilation
{
	/// <summary>
	/// Prostý expression-builder pro výrazy v podobì &lt;%$ Expression: MujVyraz %&gt;.
	/// </summary>
	/// <example>
	/// Do webové aplikace zavedeme pomocí web.config:<br/>
	/// <code>
	/// &lt;compilation&gt;<br/>
	///		&lt;expressionBuilders&gt;<br/>
	///			&lt;add expressionPrefix="Expression" type="Havit.Web.Compilation.CodeExpressionBuilder, Havit.Web" /&gt;<br/>
	///		&lt;/expressionBuilders&gt;<br/>
	///	&lt;/compilation&gt;<br/>
	/// </code>
	/// Ve stránce používáme již jako klasický expression:<br/>
	/// <code>
	/// &lt;asp:TextBox ID="NoveHesloTB" TextMode="Password" MaxLength="&lt;%$ Expression: Uzivatel.Properties.Password.MaximumLength %&gt;" runat="server" /&gt;<br/>
	/// </code>
	/// </example>
	[ExpressionPrefix("Expression")]
	public class CodeExpressionBuilder : ExpressionBuilder
	{
		/// <summary>
		/// Vrací kód, který se použije namísto vyhodnocovaného výrazu pøi kompilaci.
		/// </summary>
		/// <param name="entry">objekt reprezentující informace o property, na kterou se výraz navazuje</param>
		/// <param name="parsedData">objekt obsahující parsovaná data vrácená metodou ParseExpression</param>
		/// <param name="context">kontext</param>
		/// <returns>CodeExpression pro pøiøezení k property</returns>
		public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			return new CodeSnippetExpression(entry.Expression);
		}
	}
}
