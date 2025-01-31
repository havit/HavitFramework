﻿namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Argumenty události ScriptSubstituing.
/// </summary>
public class ClientScriptSubstituingEventArgs : EventArgs
{
	/// <summary>
	/// Konstuktor.
	/// </summary>
	/// <param name="clientScript">Klientský skript k substituci.</param>
	public ClientScriptSubstituingEventArgs(string clientScript)
	{
		this.clientScript = clientScript;
	}

	/// <summary>
	/// Klientský skript k substituci.
	/// </summary>
	public string ClientScript
	{
		get { return clientScript; }
		set { clientScript = value; }
	}
	private string clientScript;
}
