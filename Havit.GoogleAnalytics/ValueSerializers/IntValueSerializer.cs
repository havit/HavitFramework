using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers;

    internal class IntValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            return value is int;
        }

        public string Serialize(object value)
        {
            int intValue = (int)value;
            return intValue.ToString("D", CultureInfo.InvariantCulture);
        }
    }
