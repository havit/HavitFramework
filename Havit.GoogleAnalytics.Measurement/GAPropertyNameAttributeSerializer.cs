using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Default serializer
    /// </summary>
    public class GAPropertyNameAttributeSerializer : IGAModelSerializer
    {
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
                if (TrySerializeValue(model, property, out KeyValuePair<string, string>? item))
                {
                    itemList.Add(item.Value);
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
        /// <param name="item">Out parameter of with the serialized key-value pair</param>
        /// <returns>Returns true if value was serialized successfully, otherwise false</returns>
        protected bool TrySerializeValue<TModel>(TModel model, PropertyInfo propertyInfo, out KeyValuePair<string, string>? item)
        {
            ParameterNameAttribute propertyNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>(true);
            if (propertyNameAttribute == null)
            {
                item = null;
                return false;
            }

            string name = propertyNameAttribute.Name;
            string value;
            if (propertyInfo.PropertyType.IsEnum)
            {
                value = SerializeEnum((Enum)propertyInfo.GetValue(model));
            }
            else
            {
                value = SerializeValue(propertyInfo.GetValue(model));
            }

            if (value == null)
            {
                item = null;
                return false;
            }

            item = new KeyValuePair<string, string>(name, value);
            return true;
        }

        /// <summary>
        /// Serialize enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Serialized enum value</returns>
        protected virtual string SerializeEnum(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            ParameterValueAttribute attribute = type.GetField(name)
                .GetCustomAttribute<ParameterValueAttribute>();

            return attribute?.Value;
        }

        /// <summary>
        /// Serialize supplied value into string
        /// </summary>
        /// <param name="value">Supplied value to serialize</param>
        /// <returns>Serialized value</returns>
        protected virtual string SerializeValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is int intValue)
            {
                return intValue.ToString("D", CultureInfo.InvariantCulture);
            }
            else if (value is decimal decimalValue)
            {
                return decimalValue.ToString("F", CultureInfo.InvariantCulture);
            }
            else if (IsNullableType(value.GetType()))
            {
                return GetValueFromNullable(value);
            }

            return value.ToString();
        }

        /// <summary>
        /// Returns true if the type is a nullable type
        /// </summary>
        /// <param name="type">Type, possibly nullable</param>
        /// <returns>True if <paramref name="type"/> is nullable, otherwise false</returns>
        protected virtual bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Get value from a nullable type
        /// </summary>
        /// <param name="nullableValue">Instance of a nullable type</param>
        /// <returns>Serialized value</returns>
        protected virtual string GetValueFromNullable(object nullableValue)
        {
            if (nullableValue == null)
            {
                return null;
            }

            PropertyInfo valueInfo = nullableValue.GetType().GetProperty("Value");
            object unboxedValue = valueInfo.GetValue(nullableValue);
            return SerializeValue(unboxedValue);
        }
    }
}
