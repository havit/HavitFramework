using System;

namespace Havit.Text.RegularExpressions
{
	/// <summary>
	/// Typické vyhledávací vzory pro regulární výrazy.
	/// </summary>
	public sealed class RegexPatterns
	{
		/// <summary>
		/// Pattern pro kontrolu bìžného e-mailu:
		/// <list type="bullet">
		///		<item>povoleny jsou pouze znaky anglické abecedy, teèky, podtržítka, pomlèky a plus</item>
		///		<item>dva rùzné symboly nesmí následovat po sobì, stejné (s výjimkou teèky) mohou [test--test@test.com] projde, [test..test@test.com] neprojde</item>
		///		<item>nesmí zaèínat symbolem</item>
		///		<item>TLD musí mít 2-6 znakù (.museum)</item>
		///		<item>v doménì smí být teèky a pomlèky, ale nesmí následovat</item>
		///		<item>nepøíjímá IP adresy</item>
		///		<item>nepøijímá rozšíøený syntax typu [Petr Novak &lt;novak@test.com&gt;]</item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// http://www.regexlib.com/REDetails.aspx?regexp_id=295
		/// </remarks>
		// JK: Fix defectu 2011:
		//public const string EmailStrict = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+"
		//                                + @"@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";
		public const string EmailStrict = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.?)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+"
										+ @"@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";
		
		/// <summary>
		/// Pattern pro kontrolu identifikátorù.
		/// Identifikátor musí zaèínat písmenem nebo podtržítkem, nesledovat mohou i èíslice.
		/// </summary>
		public const string Identifier = @"^[a-zA-Z_]{1}[a-zA-Z0-9_]+$";

		/// <summary>
		/// Pattern pro kontrolu èasu. 24-hodinnový formát, odìlovaè dvojteèka, nepovinné vteøiny. Napø. 23:59:00.
		/// Nepøijímá 24:00.
		/// </summary>
		public const string Time24h = @"^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$";

		/// <summary>
		/// Pattern pro kontrolu IP adresy v4.
		/// </summary>
		public const string IPAddress = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";

		/// <summary>
		/// Pattern pro ovìøení celých èísel.
		/// </summary>
		/// <remarks>
		/// Pøijímá: [1], [+15], [0], [-10], [+0]<br/>
		/// Odmítá: [1.0], [abc], [+], [1,15]
		/// </remarks>
		public const string Integer = @"^[-+]?\d+$";

		#region private constructor
		/// <summary>
		/// private constructor k zabránìní instanciace statické tøídy.
		/// </summary>
		private RegexPatterns() {}
		#endregion
	}
}
