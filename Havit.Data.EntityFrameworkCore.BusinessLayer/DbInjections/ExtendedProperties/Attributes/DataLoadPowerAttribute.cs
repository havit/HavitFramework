using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
    public class DataLoadPowerAttribute : DbInjectionExtendedPropertiesAttribute
    {
        public override string ObjectType { get; } = "PROCEDURE";

        public string Value { get; set; }

        public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
        {
            return new Dictionary<string, string>
            {
                { "DataLoadPower", Value }
            };
        }
    }
}