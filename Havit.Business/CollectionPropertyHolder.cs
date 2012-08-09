using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Tøída pro objekt, který nese kolekci property BusinessObjectu.
	/// </summary>
	/// <typeparam name="CollectionType">typ kolekce, jíž je CollectionPropertyHolder nosièem</typeparam>
	/// <typeparam name="BusinessObjectType">typ prvku kolekce</typeparam>
	public class CollectionPropertyHolder<CollectionType, BusinessObjectType>: PropertyHolderBase
		where BusinessObjectType : BusinessObjectBase
		where CollectionType: BusinessObjectCollection<BusinessObjectType>, new()		
	{
		#region Constructors
		/// <summary>
		/// Založí instanci CollectionPropertyHolderu.
		/// </summary>
		/// <param name="owner">objekt, kterému CollectionPropertyHolder patøí</param>
		public CollectionPropertyHolder(BusinessObjectBase owner)
			: base(owner)
		{
		}
		#endregion

		#region Value
		/// <summary>
		/// Hodnota, kterou CollectionPropertyHolder nese.
		/// </summary>
				public CollectionType Value
		{
			get
			{
				InitializationCheck();
				return _value;
			}
		}
		private CollectionType _value;
		#endregion

		#region Initialize
		/// <summary>
		/// Inicializuje obsaženou kolekci.
		/// </summary>
		public void Initialize()
		{
			_value = new CollectionType();
			IsInitialized = true;
			_value.CollectionChanged += delegate(object sender, EventArgs e)
			{
				IsDirty = true;
			};
		}
		#endregion
	}
}
