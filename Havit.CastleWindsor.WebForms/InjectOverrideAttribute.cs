using System;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Controls behavior of sub-dependcies resolving by specifing specific names of implementations to resolve
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]

	public class InjectOverrideAttribute : Attribute
	{

		/// <summary>
		/// Name of the property (subdependency) to resolve
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// The Windsor key of the implementation
		/// </summary>
		public string DependencyKey { get; set; }

		/// <summary>
		/// Type of the property (subdependency) to resolve
		/// </summary>
		public Type DependencyServiceType { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public InjectOverrideAttribute(string propertyName, string dependencyKey, Type dependencyServiceType)
		{
			PropertyName = propertyName;
			DependencyKey = dependencyKey;
			DependencyServiceType = dependencyServiceType;
		}

	}
}
