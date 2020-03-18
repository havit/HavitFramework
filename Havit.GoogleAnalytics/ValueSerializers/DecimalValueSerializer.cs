using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers
{
    internal class DecimalValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            return value is decimal;
        }

        public string Serialize(object value)
        {
            decimal decimalValue = (decimal)value;
            return decimalValue.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}
