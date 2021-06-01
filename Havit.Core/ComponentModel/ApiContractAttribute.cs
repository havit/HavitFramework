using System;

namespace Havit.ComponentModel
{
	/// <summary>
	/// Marks interfaces which should be published/consumed as API.
	/// Used for automatic registration of gRPC endpoints (Web.Server) and gRPC clients (Web.Client).
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	public class ApiContractAttribute : Attribute
	{
		/// <summary>
		/// Indicates whether the API requires authorization (e.g. authorization token) or not (allows anonymous access). Default is <c>true</c>.
		/// </summary>
		public bool RequireAuthorization { get; set; } = true;
	}
}