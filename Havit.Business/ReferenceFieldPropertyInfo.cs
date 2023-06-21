using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Havit.Business;

/// <summary>
/// Reprezentuje sloupec v databázi,
/// který je referencí na jiný typ (je cizím klíčem do jiné tabulky).
/// </summary>
public class ReferenceFieldPropertyInfo : FieldPropertyInfo
{
	/// <summary>
	/// Typ, jenž property nese.
	/// </summary>
	public Type TargetType
	{
		get { return targetType; }
	}
	private Type targetType;

	/// <summary>
	/// Delegát na metodu vracející objekt na základě ID.
	/// </summary>
	public ObjectInfo TargetObjectInfo
	{
		get
		{
			return targetObjectInfo;
		}
	}
	private ObjectInfo targetObjectInfo;

	/// <summary>
	/// Inicializuje instanci sloupce.
	/// </summary>
	/// <param name="owner">Nadřazený objectInfo.</param>
	/// <param name="propertyName">Název property.</param>
	/// <param name="fieldName">Název sloupce v databázy.</param>
	/// <param name="isPrimaryKey">Indikuje, zda je sloupec primárním klíčem</param>
	/// <param name="fieldType">Typ databázového sloupce.</param>
	/// <param name="nullable">Indukuje, zda je povolena hodnota null.</param>
	/// <param name="maximumLength">Maximální délka dat databázového sloupce.</param>		
	/// <param name="targetType">Typ, jenž property nese.</param>
	/// <param name="targetObjectInfo">ObjectInfo na typ, jenž property nese.</param>
	[SuppressMessage("SonarLint", "S1117", Justification = "Není chybou mít parametr metody stejného jména ve třídě.")]
	public void Initialize(ObjectInfo owner, string propertyName, string fieldName, bool isPrimaryKey, SqlDbType fieldType, bool nullable, int maximumLength, Type targetType, ObjectInfo targetObjectInfo)
	{
		Initialize(owner, propertyName, fieldName, isPrimaryKey, fieldType, nullable, maximumLength);
		this.targetType = targetType;
		this.targetObjectInfo = targetObjectInfo;
	}
}
