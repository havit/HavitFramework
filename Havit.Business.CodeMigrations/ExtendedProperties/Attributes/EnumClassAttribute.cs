﻿using System;
using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EnumClassAttribute : ExtendedPropertiesAttribute
	{
		private readonly Dictionary<string, string> props = new Dictionary<string, string>
		{
			{ "EnumMode", "EnumClass" }
		};

		public string EnumPropertyName { get; }

		public override IDictionary<string, string> ExtendedProperties => props;

		public EnumClassAttribute()
		{
		}

		public EnumClassAttribute(string enumPropertyName)
		{
			EnumPropertyName = enumPropertyName;
			props.Add("EnumPropertyNameField", enumPropertyName);
		}
	}
}