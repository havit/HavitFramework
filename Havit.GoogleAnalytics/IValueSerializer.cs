using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Serializer for Google Analytics
    /// </summary>
    public interface IValueSerializer
    {
        /// <summary>
        /// Returns true, if the <paramref name="value"/> instance can be serialized.
        /// </summary>
        /// <param name="value">Value that should be serialized. Should never be null.</param>
        /// <returns>True when the current serializer can serialize the value</returns>
        bool CanSerialize(object value);

        /// <summary>
        /// Serializes the <paramref name="value"/> into string.
        /// </summary>
        /// <param name="value">Value that should be serialized. Should never be null.</param>
        /// <returns>Serialized <paramref name="value"/> into string</returns>
        string Serialize(object value);
    }
}
