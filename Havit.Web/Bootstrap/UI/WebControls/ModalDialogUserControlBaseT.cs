﻿using System.Diagnostics.CodeAnalysis;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Extends ModalDialogUserControlBase by persisting dialog Result value.
/// </summary>
[SuppressMessage("StyleCop.Analyzers", "SA1649", Justification = "Máme i negenerickou variantu (viz předek), která je v požadovaném souboru.")]
public class ModalDialogUserControlBase<T> : ModalDialogUserControlBase
{
	/// <summary>
	/// Sets result value to type T default value.
	/// </summary>
	protected override void OnDialogShown(EventArgs eventArgs)
	{
		base.OnDialogShown(eventArgs);
		Result = default(T);
	}

	/// <summary>
	/// User result of modal dialog activity.
	/// Result value is automatically reset when the dialog in shown, it is set to the type T default value.
	/// Value is persisted in ViewState (so it must be serializable).
	/// </summary>
	public virtual T Result
	{
		get
		{
			return (T)(ViewState["Result"] ?? default(T));
		}
		set
		{
			ViewState["Result"] = value;
		}
	}
}
