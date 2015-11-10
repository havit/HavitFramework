using Havit.Diagnostics.Contracts;
using Havit.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Havit
{
	/// <summary>
	/// Rozšiřující funkčnost analogická k <see cref="System.Web.UI.DataBinder"/>. Navím umožňuje hodnotu do objektu i nastavit.
	/// </summary>
	/// <remarks>
	/// Používá se např. pro resolvování BoundFieldExt.DataField, DropDownListExt.DataTextField, ...
	/// </remarks>
	public static class DataBinderExt
	{
		#region Private consts (readonly) fields
		private static readonly char[] indexExprStartChars = new char[] { '[', '(' };
		#endregion

		#region GetValue
		/// <summary>
		/// Získá hodnotu z předaného objektu a dataField.
		/// Vyhodnocuje s ohledem na &quot;tečkovou&quot; notaci, tedy například z objektu třídy Subjekt dokáže vrátit &quot;HlavniAdresa.Ulice&quot;.
		/// Pokud je dataItem <c>null</c> nebo <c>DBNull.Value</c>, vrací se tato hodnota.
		/// Pokud se při vyhodnocování &quot;po cestě&quot; získá <c>null</c> nebo <c>DBNull.Value</c>, vrací se <c>null</c>/<c>DBNull.Value</c>
		/// (příklad: Při vyhodnocování HlavniAdresa.Ulice je HlavniAdresa <c>null</c>, pak je výsledkem metody <c>null</c>.
		/// </summary>
		/// <param name="dataItem">Datový objekt, ze kterého se získává hodnota</param>
		/// <param name="dataField">Vlastnost, jejíž hodnota je získávána.</param>
		/// <returns>Hodnota, pokud se ji podařilo získat; jinak <c>null</c> nebo <c>DBNull.Value</c>.</returns>
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

					// tohle skoro nic nestojí
					System.ComponentModel.PropertyDescriptor descriptor = properties.Find(expression, true);

					if (descriptor == null)
					{
						// standardní DataBinder vyhazuje HttpException, já nechci měnit typy výjimek, pro případně try/catch.
						throw new HttpException(String.Format("Nepodařilo se vyhodnotit výraz '{0}', typ '{1}' neobsahuje vlastnost '{2}'.", dataField, currentDataItem.GetType().FullName, expression));
					}
					currentDataItem = descriptor.GetValue(currentDataItem);
				}
				else
				{
					// tohle se snad nikde nepoužívá, proto neoptimalizuji
					currentDataItem = DataBinder.GetIndexedPropertyValue(currentDataItem, expression);
				}
			}

			return currentDataItem;
		}

		/// <summary>
		/// Získá hodnotu pro zobrazení z předaného objektu a dataField a zformtátuje ji.
		/// Viz DataBinderExt.GetValue(object dataItem, string dataField).
		/// </summary>
		/// <param name="dataItem">Datový objekt, ze kterého se získává hodnota</param>
		/// <param name="dataField">Vlastnost, jejíž hodnota je získávána.</param>
		/// <param name="format">Formátovací řetězec.</param>
		/// <returns>Hdnota, pokud se ji podařilo získat a zformátovat; jinak <c>String.Empty</c>.</returns>
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
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví předanou hodnotu do předaného objektu a dataField.
		/// Vyhodnocuje s ohledem na &quot;tečkovou&quot; notaci, tedy například objektu třídy Subjekt dokáže nastavit &quot;HlavniAdresa.Ulice&quot;.
		/// Pokud je dataItem <c>null</c> nebo <c>DBNull.Value</c>, je vyhozena výjimka.
		/// Pokud se při vyhodnocování &quot;po cestě&quot; získá <c>null</c> nebo <c>DBNull.Value</c>, je vyhozena výjimka.
		/// (příklad: Při vyhodnocování HlavniAdresa.Ulice je HlavniAdresa <c>null</c>, pak je vyhozena výjimka.
		/// </summary>
		/// <param name="dataItem">Datový objekt, do kterého se nastavuje hodnota.</param>
		/// <param name="dataField">Vlastnost, jejíž hodnota je nastavována.</param>
		/// <param name="value">Nastavovaná hodnota.</param>
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
					throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', část {1} obsahuje null.", dataField, expressionGetPart));
				}

				if (currentDataItem == DBNull.Value)
				{
					throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', část {1} obsahuje DBNull.Value.", dataField, expressionGetPart));
				}
			}

			string expressionSet = expressionParts[expressionParts.Length - 1];
			if (expressionSet.IndexOfAny(indexExprStartChars) >= 0)
			{
				throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', v části {1} je nepodporovaný znak.", dataField, expressionSet));
			}

			PropertyDescriptorCollection properties = GetValueTypeProperties(currentDataItem);

			// tohle skoro nic nestojí
			System.ComponentModel.PropertyDescriptor descriptor = properties.Find(expressionSet, true);
			if (descriptor == null)
			{
				// standardní DataBinder vyhazuje HttpException, já nechci měnit typy výjimek, pro případně try/catch.
				throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', typ '{1}' neobsahuje vlastnost '{2}'.", dataField, currentDataItem.GetType().FullName, expressionSet));
			}

			if (typeof(IDataBinderExtSetValue).IsAssignableFrom(descriptor.PropertyType))
			{
				IDataBinderExtSetValue dataBinderExtSetValue = (IDataBinderExtSetValue)GetValue(currentDataItem, expressionSet);
				if (dataBinderExtSetValue == null)
				{
					throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', část {1} má obsahovat hodnotu IDataBinderExtSetValue, ale obsahuje hodnotu null.", dataField, expressionSet));
				}

				try
				{
					dataBinderExtSetValue.SetValue(value);
				}
				catch (NotSupportedException)
				{
					throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', IDataBinderExtSetValue nezpracoval nastavovanou hodnotu.", dataField));
				}
			}
			else
			{
				if (descriptor.IsReadOnly)
				{
					throw new HttpException(String.Format("Nepodařilo se nastavit hodnotu pro výraz '{0}', vlastnost '{2}' typu '{1}' je pouze ke čtení.", dataField, currentDataItem.GetType().FullName, expressionSet));
				}

				object targetValue;
				if (!Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(value, descriptor.PropertyType, out targetValue))
				{
					throw new HttpException(String.Format(value == null ? "Nepodařilo se nastavit hodnotu pro výraz '{0}', hodnotu null se nepodařilo převést na typ '{3}'." : "Nepodařilo se nastavit hodnotu pro výraz '{0}', hodnotu '{1}' typu '{2}' se nepodařilo převést na typ '{3}'.",
						dataField, // 0
						value == null ? null : value.ToString(), // 1
						value == null ? null : value.GetType().FullName, // 2
						descriptor.PropertyType.FullName)); // 3
				}

				descriptor.SetValue(currentDataItem, targetValue);
			}
		}
		#endregion

		#region SetValues
		/// <summary>
		/// Nastaví předané hodnoty do předaného objektu.
		/// Není zamýšleno pro použití v programátorském kódu. Voláno z frameworkových controlů.
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
		#endregion

		#region GetValueTypeProperties
		private static PropertyDescriptorCollection GetValueTypeProperties(object value)
		{
			System.ComponentModel.PropertyDescriptorCollection properties;

			if (value is ICustomTypeDescriptor)
			{
				// pro typu implementující ICustomTypeDescriptor (DataViewRow, atp.) nemůžeme properties cachovat
				properties = System.ComponentModel.TypeDescriptor.GetProperties(value);
			}
			else
			{
				// pro ostatní (tj. běžné) typy můžeme cachovat properties
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
		#endregion

	}
}
