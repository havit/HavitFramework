using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;

namespace Havit
{
	/// <summary>
	/// Rozšiřující funkčnost analogická k <see cref="System.Web.UI.DataBinder"/>.
	/// </summary>
	/// <remarks>
	/// Používá se např. pro resolvování BoundFieldExt.DataField, DropDownListExt.DataTextField, ...
	/// </remarks>
	public static class DataBinderExt
	{
		#region GetValue
		/// <summary>
		/// Získá hodnotu pro zobrazení z předaného objektu a dataField.
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

			object currentValue = dataItem;

			int i = 0;
			int lastExpressionIndex = expressionParts.Length - 1;
			for (i = 0; i <= lastExpressionIndex; i++)
			{
				if (currentValue == null)
				{
					return null;
				}

				if (currentValue == DBNull.Value)
				{
					return DBNull.Value;
				}

				string expression = expressionParts[i];

				if (expression.IndexOfAny(indexExprStartChars) < 0)
				{
					System.ComponentModel.PropertyDescriptorCollection properties;

					if (currentValue is ICustomTypeDescriptor)
					{
						// pro typu implementující ICustomTypeDescriptor (DataViewRow, atp.) nemůžeme properties cachovat
						properties = System.ComponentModel.TypeDescriptor.GetProperties(currentValue);
					}
					else
					{
						// pro ostatní (tj. běžné) typy můžeme cachovat properties
						Type currentType = currentValue.GetType();
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

					System.ComponentModel.PropertyDescriptor descriptor = properties.Find(expression, true);
					// tohle skoro nic nestojí
					if (descriptor == null)
					{
						// standardní DataBinder vyhazuje HttpException, já nechci měnit typy výjimek, pro případně try/catch.
						throw new HttpException(String.Format("Nepodařilo se vyhodnotit výraz '{0}', typ '{1}' neobsahuje vlastnost '{2}'.", dataField, currentValue.GetType().FullName, expression));

					}
					currentValue = descriptor.GetValue(currentValue);
				}
				else
				{
					// tohle se snad nikde nepoužívá, proto neoptimalizuji
					currentValue = DataBinder.GetIndexedPropertyValue(currentValue, expression);
				}
			}

			return currentValue;
		}
		private static Dictionary<Type, System.ComponentModel.PropertyDescriptorCollection> getValuePropertiesCache = new Dictionary<Type, System.ComponentModel.PropertyDescriptorCollection>();
		private static readonly char[] indexExprStartChars = new char[] { '[', '(' };

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
	}
}
