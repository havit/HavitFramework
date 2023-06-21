using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Předek pro vlastní user controly, 
/// které jsou zobrazovány jako dialog prostřednictvím zapouzdřeného AjaxModalDialogu.
/// </summary>
public abstract class ModalDialogUserControlBase : UserControl
{
	/// <summary>
	/// Udává, zda je dialog viditelný.
	/// </summary>
	protected bool DialogVisible
	{
		get
		{
			return GetModalDialogControl().DialogVisible;
		}
	}

	/// <summary>
	/// OnInit.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		ModalDialogBase modalDialog = GetModalDialogControl();
		modalDialog.DialogShowing += ModalDialog_DialogShowing;
		modalDialog.DialogShown += MainWebModalDialog_DialogShown;
		modalDialog.DialogHiding += ModalDialog_DialogHiding;
		modalDialog.DialogHidden += MainWebModalDialog_DialogHidden;
	}

	/// <summary>
	/// Vrací zapouzdřený ModalDialog zajišťující zobrazování a schovávání obsahu dialogu.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected abstract ModalDialogBase GetModalDialogControl();

	/// <summary>
	/// Událost oznamující před zobrazením dialogu.
	/// </summary>
	public event CancelEventHandler DialogShowing;

	/// <summary>
	/// Událost oznamující zobrazení dialogu.
	/// </summary>
	public event EventHandler DialogShown;

	/// <summary>
	/// Událost oznamující před skrytím dialogu.
	/// </summary>
	public event CancelEventHandler DialogHiding;

	/// <summary>
	/// Událost oznamující skrytí dialogu.
	/// </summary>
	public event EventHandler DialogHidden;

	private void ModalDialog_DialogShowing(object sender, CancelEventArgs e)
	{
		OnDialogShowing(e);
	}

	private void MainWebModalDialog_DialogShown(object sender, EventArgs e)
	{
		OnDialogShown(e);
	}

	private void ModalDialog_DialogHiding(object sender, CancelEventArgs e)
	{
		OnDialogHiding(e);
	}

	private void MainWebModalDialog_DialogHidden(object sender, EventArgs e)
	{
		OnDialogHidden(e);
	}

	/// <summary>
	/// Zobrazí dialog.
	/// </summary>
	public void Show()
	{
		GetModalDialogControl().Show();
	}

	/// <summary>
	/// Skryje dialog.
	/// </summary>
	public void Hide()
	{
		GetModalDialogControl().Hide();
	}

	/// <summary>
	/// Obsluhuje událost před zobrazením dialogu.
	/// </summary>
	protected virtual void OnDialogShowing(CancelEventArgs eventArgs)
	{
		if (DialogShowing != null)
		{
			DialogShowing(this, eventArgs);
		}
	}

	/// <summary>
	/// Obsluhuje událost zobrazení dialogu.
	/// </summary>
	protected virtual void OnDialogShown(EventArgs eventArgs)
	{
		if (DialogShown != null)
		{
			DialogShown(this, eventArgs);
		}
	}

	/// <summary>
	/// Obsluhuje událost před před skrytím dialogu.
	/// </summary>
	protected virtual void OnDialogHiding(CancelEventArgs eventArgs)
	{
		if (DialogHiding != null)
		{
			DialogHiding(this, eventArgs);
		}
	}

	/// <summary>
	/// Obsluhuje událost skrytí dialogu.
	/// </summary>
	protected virtual void OnDialogHidden(EventArgs eventArgs)
	{
		if (DialogHidden != null)
		{
			DialogHidden(this, eventArgs);
		}
	}
}
