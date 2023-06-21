using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics;

    /// <summary>
    /// Attribute that defines key for the property it is used on
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterNameAttribute : Attribute
    {
        /// <summary>
        /// Key that is serialized into a query parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="name">Key that is serialized into a query parameter</param>
        public ParameterNameAttribute(string name)
        {
            Name = name;
        }
    }
