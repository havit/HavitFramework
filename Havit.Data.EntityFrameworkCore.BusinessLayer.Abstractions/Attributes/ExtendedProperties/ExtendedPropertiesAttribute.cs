﻿using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// Bázová třída k atributům, jejichž umístěním k vlastnosti nebo třídě chceme říct, že se k databázovému sloupci či databázové tabulce má vytvořit extended property.
/// </summary>
public abstract class ExtendedPropertiesAttribute : Attribute
{
	/// <summary>
	/// Vrací extended properties, které mají být založeny.
	/// </summary>
	public abstract IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo);
}
