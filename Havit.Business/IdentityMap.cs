using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Havit.Diagnostics.Contracts;

namespace Havit.Business
{
	/// <summary>
	/// Identity map pro business-objekty.
	/// </summary>
	public class IdentityMap
	{
		/// <summary>
		/// Hashtable obsahující hashtable pro každý typ.
		/// </summary>
		private readonly Dictionary<Type, Dictionary<int, WeakReference>> types;

		/// <summary>
		/// Vytvoří instanci třídy <see cref="IdentityMap"/>.
		/// </summary>
		public IdentityMap()
		{
			types = new Dictionary<Type, Dictionary<int, WeakReference>>();
		}

		/// <summary>
		/// Uloží business-objekt do identity-map.
		/// </summary>
		/// <param name="businessObject">business objekt</param>
		public void Store(BusinessObjectBase businessObject)
		{
			Contract.Requires<ArgumentNullException>(businessObject != null, nameof(businessObject));
			Contract.Requires<ArgumentException>(!businessObject.IsNew, "businessObject ukládaný do IdentityMap nesmí být nový.");

			Type businessObjectType = businessObject.GetType();
			Dictionary<int, WeakReference> typeDictionary;
			if (!types.TryGetValue(businessObjectType, out typeDictionary))
			{			
				typeDictionary = new Dictionary<int, WeakReference>();
				types.Add(businessObjectType, typeDictionary);
			}

			WeakReference reference;
			if (typeDictionary.TryGetValue(businessObject.ID, out reference))			
			{
				if (reference.Target != null)
				{
					if (!Object.ReferenceEquals(reference.Target, businessObject))
					{
						throw new InvalidOperationException("V IdentityMap je již jiná instance tohoto objektu.");
					}
				}
				else
				{
					reference.Target = businessObject;
				}
			}
			else
			{
				typeDictionary.Add(businessObject.ID, new WeakReference(businessObject));
			}
		}

		/// <summary>
		/// Načte business-objekt z identity-map.
		/// </summary>
		/// <typeparam name="T">typ business objektu</typeparam>
		/// <param name="id">ID business objektu</param>
		/// <param name="target">cíl, kam má být business-objekt načten</param>
		/// <returns><c>true</c>, pokud se podařilo načíst; <c>false</c>, pokud objekt v identity-map není (target pak obsahuje <c>null</c>)</returns>
		public bool TryGet<T>(int id, out T target)
			where T : BusinessObjectBase
		{
			Dictionary<int, WeakReference> typeDictionary;
			if (!types.TryGetValue(typeof(T), out typeDictionary))
			{
				target = null;
				return false;
			}

			WeakReference reference;
			if (!typeDictionary.TryGetValue(id, out reference))
			{
				target = null;
				return false;
			}

			target = (T)reference.Target;
			return (target != null);
		}

		/// <summary>
		/// Vrátí business-objekt z identity-map.
		/// </summary>
		/// <typeparam name="T">typ business objektu</typeparam>
		/// <param name="id">ID business objektu</param>
		/// <returns>business-objekt z identity-map; <c>null</c>, pokud v ní není</returns>
		public T Get<T>(int id)
			where T : BusinessObjectBase
		{
			Dictionary<int, WeakReference> typeDictionary;
			if (!types.TryGetValue(typeof(T), out typeDictionary))
			{
				return null;
			}

			WeakReference reference;
			if (!typeDictionary.TryGetValue(id, out reference))
			{
				return null;
			}

			return (T)reference.Target;
		}
	}
}
