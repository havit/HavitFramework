﻿using System;

namespace Havit.Web.CastleWindsor.WebForms
{
	/// <summary>
	/// Marks properties that shall be injected in WebForms pages and user controls.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class InjectAttribute : Attribute
	{
	}
}
