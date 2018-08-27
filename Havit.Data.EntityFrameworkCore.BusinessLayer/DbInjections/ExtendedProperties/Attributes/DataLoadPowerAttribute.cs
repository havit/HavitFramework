using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	/// <summary>
	/// Atribút pre nastavenie DataLoadPower extended property na uloženej procedúre.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
    public class DataLoadPowerAttribute : DbInjectionExtendedPropertiesAttribute
    {
	    /// <inheritdoc />
	    public override string ObjectType { get; } = "PROCEDURE";

		/// <summary>
		/// Určuje, jaké množství dat se vrací z uložené procedury. Je použito pro návratové typy Object a Collection (viz ResultType).
		/// </summary>
		public DataLoadPowerType PowerType { get; }

		/// <summary>
		/// Konštruktor.
		/// </summary>
	    public DataLoadPowerAttribute(DataLoadPowerType powerType)
	    {
		    PowerType = powerType;
	    }

	    /// <inheritdoc />
	    public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
	    {
		    { "DataLoadPower", PowerType.ToString() }
	    };
    }
}