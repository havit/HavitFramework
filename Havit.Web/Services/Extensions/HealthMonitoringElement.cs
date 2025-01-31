using System.ServiceModel.Configuration;

namespace Havit.Web.Services.Extensions;

/// <summary>
/// Reprezentuje konfigurační WCF element, který je použit pro zapnutí health monitoring behavior pomocí web.config.
/// </summary>
public class HealthMonitoringElement : BehaviorExtensionElement
{
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
}