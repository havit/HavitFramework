using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Services.Extensions
{
	/// <summary>
	/// Reprezentuje konfigurační WCF element, který je použit pro zapnutí health monitoring behavior pomocí web.config.
	/// </summary>
	public class HealthMonitoringElement : BehaviorExtensionElement
	{
		#region BehaviorType
		/// <summary>
		/// Gets the type of behavior.
		/// </summary>
		/// <returns>
		/// The type of behavior.
		/// </returns>
		public override Type BehaviorType
		{
			get { return typeof(HealthMonitoringBehaviorAttribute); }
		}
		#endregion

		#region CreateBehavior
		/// <summary>
		/// Creates a behavior extension based on the current configuration settings.
		/// </summary>
		/// <returns>
		/// The behavior extension.
		/// </returns>
		protected override object CreateBehavior()
		{
			return new HealthMonitoringBehaviorAttribute();
		}
		#endregion
	}
}