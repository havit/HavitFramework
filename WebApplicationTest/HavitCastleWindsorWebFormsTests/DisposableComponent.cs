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
		private readonly Guid _instanceId;
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
			return String.Format("Hello I am type of {0} and my name is {1}", GetType().Name, _instanceId);
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