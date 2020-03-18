using Havit.GoogleAnalytics.ValueSerializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Static holder for collection of default <see cref="IValueSerializer"/>
    /// </summary>
    public static class ValueSerializerCollection
    {
        /// <summary>
        /// Collection of default <see cref="IValueSerializer"/>
        /// </summary>
        public static IValueSerializer[] GetDefaultValueSerializers()
        {
            return new IValueSerializer[]
            {
                new StringValueSerializer(),
                new BooleanValueSerializer(),
                new IntValueSerializer(),
                new DecimalValueSerializer(),
                new EnumValueSerializer(),
                new NullableValueSerializer()
            };
        }
    }
}
