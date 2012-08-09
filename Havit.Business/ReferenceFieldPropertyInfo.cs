using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;
using System.Data;

namespace Havit.Business
{
	/// <summary>
	/// Reprezentuje sloupec v databázi,
	/// který je referencí na jiný typ (je cizím klíèem do jiné tabulky).
	/// </summary>
	[Serializable]
	public class ReferenceFieldPropertyInfo : FieldPropertyInfo
	{
		/// <summary>
		/// Vytvoøí instanci sloupce.
		/// </summary>
		/// <param name="fieldName">Název sloupce v databázy.</param>
		/// <param name="isPrimaryKey">Indikuje, zda je sloupec primárním klíèem</param>
		/// <param name="nullable">Indukuje, zda je povolena hodnota null.</param>
		/// <param name="fieldType">Typ databázového sloupce.</param>
		/// <param name="maximumLength">Maximální délka dat databázového sloupce.</param>		
		/// <param name="memberType">Typ, jenž property nese.</param>
		/// <param name="getObjectMethod">Delegát na metodu vracející objekt na základì ID.</param>
		public ReferenceFieldPropertyInfo(string fieldName, bool isPrimaryKey, SqlDbType fieldType, bool nullable, int maximumLength, Type memberType, GetObjectDelegate getObjectMethod)
			: base(fieldName, isPrimaryKey, fieldType, nullable, maximumLength)
		{
			this.memberType = memberType;
			this.getObjectMethod = getObjectMethod;
		}

		/// <summary>
		/// Typ, jenž property nese.
		/// </summary>
		public Type MemberType
		{
			get { return memberType; }
		}
		private Type memberType;

		/// <summary>
		/// Delegát na metodu vracející objekt na základì ID.
		/// </summary>
		public GetObjectDelegate GetObjectMethod
		{
			get
			{
				return getObjectMethod;
			}
		}
		private GetObjectDelegate getObjectMethod;
	}
}
