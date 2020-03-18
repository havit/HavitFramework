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
        /// Create new instance of type <see cref="PropertyNameAttributeSerializer"/>
        /// </summary>
        public PropertyNameAttributeSerializer()
        {
            this.serializers = new List<IValueSerializer>(ValueSerializerCollection.GetDefaultValueSerializers());
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
                if (TrySerializeValue(model, property, out IEnumerable<KeyValuePair<string, string>> items))
                {
                    itemList.AddRange(items);
                }
            }

            return itemList;
        }

        /// <summary>
        /// Try to serialize value using supplied model instance (<paramref name="model"/>) and property info (<paramref name="propertyInfo"/>)
        /// </summary>
        /// <typeparam name="TModel">Type of the model which contains supplied <paramref name="propertyInfo"/></typeparam>
        /// <param name="model">Model instance to serialize</param>
        /// <param name="propertyInfo">Supplied property from the model to serialize</param>
        /// <param name="items">Out parameter of with the serialized key-value pairs</param>
        /// <returns>Returns true if value was serialized successfully, otherwise false</returns>
        protected bool TrySerializeValue<TModel>(TModel model, PropertyInfo propertyInfo, out IEnumerable<KeyValuePair<string, string>> items)
        {
            object propertyValue = propertyInfo.GetValue(model);
            if (propertyValue == null)
            {
                items = null;
                return false;
            }

            ParameterNameAttribute propertyNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>(true);
            if (propertyNameAttribute == null)
            {
                items = null;
                return false;
            }
            string name = propertyNameAttribute.Name;

            foreach (var serializer in serializers)
            {
                if (serializer.CanSerialize(propertyValue))
                {
                    string value = serializer.Serialize(propertyValue);
                    items = new[] { new KeyValuePair<string, string>(name, value) };
                    return true;
                }
            }

            if (TrySerializeSpecialValue(name, propertyValue, out items))
            {
                return true;
            }

            items = null;
            return false;
        }

        private bool TrySerializeSpecialValue(string name, object propertyValue, out IEnumerable<KeyValuePair<string, string>> items)
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
    }
}
