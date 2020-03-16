using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Interface of the serializer for Google Analytics models
    /// </summary>
    public interface IGoogleAnalyticsModelSerializer
    {
        /// <summary>
        /// Serializes <paramref name="model"/> instance into a key-value collection
        /// </summary>
        /// <typeparam name="TModel">Type of the model instance to serialize</typeparam>
        /// <param name="model">Instance of the model to serialize</param>
        /// <returns>Serialized key-value collection of properties of the model</returns>
        List<KeyValuePair<string, string>> SerializeModel<TModel>(TModel model);
    }
}
