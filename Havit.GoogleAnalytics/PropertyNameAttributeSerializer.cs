using Havit.GoogleAnalytics.ValueSerializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Default serializer
    /// </summary>
    public class PropertyNameAttributeSerializer : IGoogleAnalyticsModelSerializer
    {
        private readonly List<IValueSerializer> serializers;

        /// <summary>
        /// Create new instance of type <see cref="PropertyNameAttributeSerializer"/> with default value serializers from <see cref="ValueSerializerCollection.GetDefaultValueSerializers"/>
        /// </summary>
        public PropertyNameAttributeSerializer()
        {
            this.serializers = new List<IValueSerializer>(ValueSerializerCollection.GetDefaultValueSerializers());
        }

        /// <summary>
        /// Create new instance of type <see cref="PropertyNameAttributeSerializer"/> with custom serializers 
        /// preffered over default serializers from <see cref="ValueSerializerCollection.GetDefaultValueSerializers"/>
        /// </summary>
        public PropertyNameAttributeSerializer(IEnumerable<IValueSerializer> customSerializers)
        {
            this.serializers = new List<IValueSerializer>(customSerializers);
            this.serializers.AddRange(ValueSerializerCollection.GetDefaultValueSerializers());
        }

        /// <summary>
        /// Serializes model instance into key-value collection
        /// </summary>
        /// <typeparam name="TModel">Type of model to serialize</typeparam>
        /// <param name="model">Model instance to serialize</param>
        /// <returns>Serialized key-value collection</returns>
        public List<KeyValuePair<string, string>> SerializeModel<TModel>(TModel model)
        {
            var itemList = new List<KeyValuePair<string, string>>();
            foreach (var property in model.GetType().GetProperties())
            {
                if (!TryGetPropertyName(property, out string name))
                {
                    continue;
                }

                if (!TryGetPropertyValue(model, property, out object propertyValue))
                {
                    continue;
                }

                IEnumerable<KeyValuePair<string, string>> items;
                if (TrySerializeValue(name, propertyValue, out items)
                    || TrySerializeSpecialValue(name, propertyValue, out items))
                {
                    itemList.AddRange(items);
                }
            }

            return itemList;
        }

        /// <summary>
        /// Try to serialize value using supplied parameter name (<paramref name="name"/>) and property value (<paramref name="propertyValue"/>)
        /// </summary>
        /// <param name="name">Parameter name for the query string</param>
        /// <param name="propertyValue">Value to be serialized into query string</param>
        /// <param name="items">Out parameter of with the serialized key-value pairs</param>
        /// <returns>Returns true if value was serialized successfully, otherwise false</returns>
        protected virtual bool TrySerializeValue(string name, object propertyValue, out IEnumerable<KeyValuePair<string, string>> items)
        {
            foreach (var serializer in serializers)
            {
                if (serializer.CanSerialize(propertyValue))
                {
                    string value = serializer.Serialize(propertyValue);
                    items = new[] { new KeyValuePair<string, string>(name, value) };
                    return true;
                }
            }

            items = null;
            return false;
        }

        /// <summary>
        /// Try to serialize value using supplied parameter name (<paramref name="name"/>) and property value (<paramref name="propertyValue"/>)
        /// </summary>
        /// <param name="name">Parameter name for the query string</param>
        /// <param name="propertyValue">Value to be serialized into query string</param>
        /// <param name="items">Out parameter of with the serialized key-value pairs</param>
        /// <returns>Returns true if value was serialized successfully, otherwise false</returns>
        protected virtual bool TrySerializeSpecialValue(string name, object propertyValue, out IEnumerable<KeyValuePair<string, string>> items)
        {
            if (propertyValue is IEnumerable<KeyValuePair<int, string>> customDimensions)
            {
                items = customDimensions.Select(x => new KeyValuePair<string, string>(name + x.Key, x.Value));
                return true;
            }
            else if (propertyValue is IEnumerable<KeyValuePair<int, int>> customMetrics)
            {
                var serializer = new IntValueSerializer();
                items = customMetrics.Select(x => new KeyValuePair<string, string>(name + x.Key, serializer.Serialize(x.Value)));
                return true;
            }

            items = null;
            return false;
        }

        private bool TryGetPropertyName(PropertyInfo propertyInfo, out string name)
        {
            ParameterNameAttribute propertyNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>(true);
            if (propertyNameAttribute == null)
            {
                name = null;
                return false;
            }

            name = propertyNameAttribute.Name;
            return true;
        }

        private bool TryGetPropertyValue<TModel>(TModel model, PropertyInfo propertyInfo, out object propertyValue)
        {
            propertyValue = propertyInfo.GetValue(model);
            if (propertyValue == null)
            {
                return false;
            }

            return true;
        }
    }
}
