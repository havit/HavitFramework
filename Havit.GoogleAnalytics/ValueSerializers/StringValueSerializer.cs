using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers
{
    internal class StringValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            return value is string;
        }

        public string Serialize(object value)
        {
            return (string)value;
        }
    }
}
