﻿using Havit.Diagnostics.Contracts;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Defines target control for assigning css class and showing tooltip when validation fails.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ValidationDisplayTargetAttribute : Attribute
{
	/// <summary>
	/// Control ID to be used as a target control in user control validation.
	/// </summary>
	public string DisplayTargetControl { get; private set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public ValidationDisplayTargetAttribute(string displayTargetControl)
	{
		Contract.Requires<ArgumentNullException>(!String.IsNullOrEmpty(displayTargetControl));

		this.DisplayTargetControl = displayTargetControl;
	}
}
