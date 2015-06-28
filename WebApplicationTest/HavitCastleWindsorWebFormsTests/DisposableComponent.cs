using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public class DisposableComponent : IDisposable, IDisposableComponent
	{
		#region Fields
		private Guid _instanceId;
		#endregion

		#region DisposableComponent
		public DisposableComponent()
		{
			_instanceId = Guid.NewGuid();
			Debug.WriteLine("Constructing instance " + _instanceId);
		}
		#endregion

		#region Hello
		public string Hello()
		{
			return "Hello, my name is " + _instanceId;
		}
		#endregion

		#region Dispose
		public void Dispose()
		{
			Debug.WriteLine("Disposing instance " + _instanceId);
		}
		#endregion
	}
}