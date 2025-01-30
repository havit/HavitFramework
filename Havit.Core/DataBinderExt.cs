using Havit.Diagnostics.Contracts;
using Havit.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Havit;

/// <summary>
/// Extends functionality similar to System.Web.UI.DataBinder. It also allows setting a value to an object.
/// </summary>
/// <remarks>
/// Used, for example, for resolving BoundFieldExt.DataField, DropDownListExt.DataTextField, ...
/// </remarks>
public static class DataBinderExt
{
	private static readonly char[] indexExprStartChars = new char[] { '[', '(' };

	/// <summary>
	/// Gets the value from the passed object and dataField.
	/// Evaluates with respect to the "dot" notation, for example, from an object of the Subjekt class, it can return "MainAddress.Street".
	/// If dataItem is <c>null</c> or <c>DBNull.Value</c>, this value is returned.
	/// If <c>null</c> or <c>DBNull.Value</c> is obtained during evaluation "along the way", <c>null</c>/<c>DBNull.Value</c> is returned
	/// (example: When evaluating MainAddress.Street, MainAddress is <c>null</c>, then the result of the method is <c>null</c>.
	/// </summary>
	/// <param name="dataItem">Data object from which the value is obtained</param>
	/// <param name="dataField">Property whose value is obtained.</param>
	/// <returns>Value if it was successfully obtained; otherwise <c>null</c> or <c>DBNull.Value</c>.</returns>
	public static object GetValue(object dataItem, string dataField)
	{
		string[] expressionParts = dataField.Split('.');

		object currentDataItem = dataItem;

		int i = 0;
		int lastExpressionIndex = expressionParts.Length - 1;
		for (i = 0; i <= lastExpressionIndex; i++)
		{
			if (currentDataItem == null)
			{
				return null;
			}

			if (currentDataItem == DBNull.Value)
			{
				return DBNull.Value;
			}

			string expression = expressionParts[i];

			if (expression.IndexOfAny(indexExprStartChars) < 0)
			{
				PropertyDescriptorCollection properties = GetValueTypeProperties(currentDataItem);

				// This almost costs nothing
				System.ComponentModel.PropertyDescriptor descriptor = properties.Find(expression, true);

				if (descriptor == null)
				{
					// The standard DataBinder throws an HttpException, I don't want to change the types of exceptions for possible try/catch.
					throw new InvalidOperationException(String.Format("Failed to evaluate the expression '{0}', type '{1}' does not contain the property '{2}'.", dataField, currentDataItem.GetType().FullName, expression));
				}
				currentDataItem = descriptor.GetValue(currentDataItem);
			}
			else
			{
				throw new InvalidOperationException(String.Format("Failed to evaluate the expression '{0}', the part {1} contains an unsupported character.", dataField, expression));
			}
		}

		return currentDataItem;
	}

	/// <summary>
	/// Gets the display value from the passed object and dataField and formats it.
	/// See DataBinderExt.GetValue(object dataItem, string dataField).
	/// </summary>
	/// <param name="dataItem">Data object from which the value is obtained</param>
	/// <param name="dataField">Property whose value is obtained.</param>
	/// <param name="format">Formatting string.</param>
	/// <returns>Value if it was successfully obtained and formatted; otherwise <c>String.Empty</c>.</returns>
	public static string GetValue(object dataItem, string dataField, string format)
	{
		object propertyValue = GetValue(dataItem, dataField);
		if ((propertyValue == null) || (propertyValue == DBNull.Value))
		{
			return String.Empty;
		}
		if (String.IsNullOrEmpty(format))
		{
			return propertyValue.ToString();
		}
		return String.Format(format, propertyValue);
	}

	/// <summary>
	/// Sets the passed value to the passed object and dataField.
	/// Evaluates with respect to the "dot" notation, for example, from an object of the Subjekt class, it can set "MainAddress.Street".
	/// If dataItem is <c>null</c> or <c>DBNull.Value</c>, an exception is thrown.
	/// If <c>null</c> or <c>DBNull.Value</c> is obtained during evaluation "along the way", an exception is thrown.
	/// (example: When evaluating MainAddress.Street, MainAddress is <c>null</c>, then an exception is thrown.
	/// </summary>
	/// <param name="dataItem">Data object to which the value is set.</param>
	/// <param name="dataField">Property whose value is set.</param>
	/// <param name="value">Value to be set.</param>
	public static void SetValue(object dataItem, string dataField, object value)
	{
		Contract.Requires(dataItem != null);
		Contract.Requires(!String.IsNullOrEmpty(dataField));

		string[] expressionParts = dataField.Split('.');

		object currentDataItem = dataItem;
		if (expressionParts.Length > 1)
		{
			string expressionGetPart = String.Join(".", expressionParts.Take(expressionParts.Length - 1).ToArray());

			currentDataItem = GetValue(currentDataItem, expressionGetPart);
			if (currentDataItem == null)
			{
				throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', the part {1} contains null.", dataField, expressionGetPart));
			}

			if (currentDataItem == DBNull.Value)
			{
				throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', the part {1} contains DBNull.Value.", dataField, expressionGetPart));
			}
		}

		string expressionSet = expressionParts[expressionParts.Length - 1];
		if (expressionSet.IndexOfAny(indexExprStartChars) >= 0)
		{
			throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', the part {1} contains an unsupported character.", dataField, expressionSet));
		}

		PropertyDescriptorCollection properties = GetValueTypeProperties(currentDataItem);

		// This almost costs nothing
		System.ComponentModel.PropertyDescriptor descriptor = properties.Find(expressionSet, true);
		if (descriptor == null)
		{
			// The standard DataBinder throws an HttpException, I don't want to change the types of exceptions for possible try/catch.
			throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', type '{1}' does not contain the property '{2}'.", dataField, currentDataItem.GetType().FullName, expressionSet));
		}

		if (typeof(IDataBinderExtSetValue).IsAssignableFrom(descriptor.PropertyType))
		{
			IDataBinderExtSetValue dataBinderExtSetValue = (IDataBinderExtSetValue)GetValue(currentDataItem, expressionSet);
			if (dataBinderExtSetValue == null)
			{
				throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', the part {1} should contain an IDataBinderExtSetValue value, but it contains a null value.", dataField, expressionSet));
			}

			try
			{
				dataBinderExtSetValue.SetValue(value);
			}
			catch (NotSupportedException)
			{
				throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', IDataBinderExtSetValue did not process the value being set.", dataField));
			}
		}
		else
		{
			if (descriptor.IsReadOnly)
			{
				throw new InvalidOperationException(String.Format("Failed to set the value for the expression '{0}', the property '{2}' of type '{1}' is read-only.", dataField, currentDataItem.GetType().FullName, expressionSet));
			}

			object targetValue;
			if (!Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(value, descriptor.PropertyType, out targetValue))
			{
				throw new InvalidOperationException(String.Format(value == null ? "Failed to set the value for the expression '{0}', failed to convert the null value to type '{3}'." : "Failed to set the value for the expression '{0}', failed to convert the value '{1}' of type '{2}' to type '{3}'.",
					dataField, // 0
					value == null ? null : value.ToString(), // 1
					value == null ? null : value.GetType().FullName, // 2
					descriptor.PropertyType.FullName)); // 3
			}

			descriptor.SetValue(currentDataItem, targetValue);
		}
	}

	/// <summary>
	/// Sets the passed values to the passed object.
	/// Not intended for use in application code. Called from framework controls.
	/// </summary>
	public static void SetValues(object dataItem, IOrderedDictionary fieldValues)
	{
		Contract.Requires(dataItem != null);
		Contract.Requires(fieldValues != null);

		foreach (DictionaryEntry item in fieldValues)
		{
			DataBinderExt.SetValue(dataItem, (string)item.Key, item.Value);
		}
	}

	private static PropertyDescriptorCollection GetValueTypeProperties(object value)
	{
		System.ComponentModel.PropertyDescriptorCollection properties;

		if (value is ICustomTypeDescriptor)
		{
			// We cannot cache properties for types implementing ICustomTypeDescriptor (DataViewRow, etc.)
			properties = System.ComponentModel.TypeDescriptor.GetProperties(value);
		}
		else
		{
			// We can cache properties for other (normal) types
			Type currentType = value.GetType();
			lock (getValuePropertiesCache)
			{
				getValuePropertiesCache.TryGetValue(currentType, out properties);
			}

			if (properties == null)
			{
				properties = System.ComponentModel.TypeDescriptor.GetProperties(currentType);
				lock (getValuePropertiesCache)
				{
					getValuePropertiesCache[currentType] = properties;
				}
			}
		}
		return properties;
	}
	private static readonly Dictionary<Type, System.ComponentModel.PropertyDescriptorCollection> getValuePropertiesCache = new Dictionary<Type, System.ComponentModel.PropertyDescriptorCollection>();
}
