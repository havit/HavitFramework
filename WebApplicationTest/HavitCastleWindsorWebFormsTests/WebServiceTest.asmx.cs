using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class WebServiceBaseTest : InjectableWebServiceBase
	{
		#region DisposableComponent
		[Inject]
		public IDisposableComponent DisposableComponent { get; set; }
		#endregion

		#region Hello
		[WebMethod]
		public string Hello()
		{
			return DisposableComponent.Hello();
		}
		#endregion
	}
}
