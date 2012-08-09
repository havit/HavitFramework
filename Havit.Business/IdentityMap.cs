using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Business
{
	/// <summary>
	/// Identity map pro business-objekty.
	/// </summary>
	public class IdentityMap
	{
		#region private fields
		/// <summary>
		/// Hashtable obsahující hashtable pro každý typ.
		/// </summary>
		private Hashtable types;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci tøídy <see cref="IdentityMap"/>.
		/// </summary>
		public IdentityMap()
		{
			types = new Hashtable();
		}
		#endregion

		#region Store<T>
		/// <summary>
		/// Uloží business-objekt do identity-map.
		/// </summary>
		/// <typeparam name="T">typ business objektu</typeparam>
		/// <param name="businessObject">business objekt</param>
		public void Store<T>(T businessObject)
			where T : BusinessObjectBase
		{
			if (businessObject == null)
			{
				throw new ArgumentNullException("businessObject");
			}

			if (businessObject.IsNew)
			{
				throw new ArgumentException("businessObject ukládaný do IdentityMap nesmí být nový.", "businessObject");
			}

			Hashtable typeHashtable = types[typeof(T)] as Hashtable;
			if (typeHashtable == null)
			{
				typeHashtable = new Hashtable();
				types.Add(typeof(T), typeHashtable);
			}

			if (typeHashtable.ContainsKey(businessObject.ID))
			{
#warning Co se má stát pøi ukládání existujícího objektu do IdentityMap?
				throw new InvalidOperationException("IdentityMap již tento objekt obsahuje.");
			}

			typeHashtable.Add(businessObject.ID, businessObject);
		}
		#endregion

		#region Get<T>
		/// <summary>
		/// Vrátí business-objekt z identity-map.
		/// </summary>
		/// <typeparam name="T">typ business objektu</typeparam>
		/// <param name="id">ID business objektu</param>
		/// <returns>business-objekt z identity-map; <c>null</c>, pokud v ní není</returns>
		public T Get<T>(int id)
			where T : BusinessObjectBase
		{
			Hashtable typeHashtable = types[typeof(T)] as Hashtable;
			if (typeHashtable == null)
			{
				return null;
			}
			return (T)typeHashtable[id];
		}
		#endregion
	}
}
