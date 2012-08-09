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

			WeakReference temp = (WeakReference)typeHashtable[businessObject.ID];
			if (temp != null)
			{
				if (temp.Target != null)
				{
					if (!Object.ReferenceEquals(temp.Target, businessObject))
					{
						throw new InvalidOperationException("V IdentityMap je již jiná instance tohoto objektu.");
					}
				}
				else
				{
					temp.Target = businessObject;
				}
			}
			else
			{
				typeHashtable.Add(businessObject.ID, new WeakReference(businessObject));
			}
		}
		#endregion

		#region TryGet<T>
		/// <summary>
		/// Naète business-objekt z identity-map.
		/// </summary>
		/// <typeparam name="T">typ business objektu</typeparam>
		/// <param name="id">ID business objektu</param>
		/// <param name="target">cíl, kam má být business-objekt naèten</param>
		/// <returns><c>true</c>, pokud se podaøilo naèíst; <c>false</c>, pokud objekt v identity-map není (target pak obsahuje <c>null</c>)</returns>
		public bool TryGet<T>(int id, out T target)
			where T : BusinessObjectBase
		{
			target = null;
			Hashtable typeHashtable = types[typeof(T)] as Hashtable;
			if (typeHashtable == null)
			{
				return false;
			}
			WeakReference reference = (WeakReference)typeHashtable[id];
			if (reference != null)
			{
				target = (T)reference.Target;
			}
			
			return !(target == null);
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
			WeakReference reference = (WeakReference)typeHashtable[id];
			if (reference == null)
			{
				return null;
			}
			return (T)reference.Target;
		}
		#endregion
	}
}
