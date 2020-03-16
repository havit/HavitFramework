using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Attribute that defines value for the enum
    /// </summary>
    public class ParameterValueAttribute : Attribute
    {
        /// <summary>
        /// Value of the enum that is serialized into a query parameter
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="value">Value of the enum that is serialized into a query parameter</param>
        public ParameterValueAttribute(string value)
        {
            Value = value;
        }
    }
}
