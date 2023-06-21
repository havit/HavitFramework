using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Messenger - třída pro sběr zpráv (informačních zpráv, upozorněnní a chybových zpráv) k zobrazení v uživatelském rozhraní.
/// </summary>
public abstract class Messenger
{
	/// <summary>
	/// Výchozí instance messengera.
	/// </summary>
	public static Messenger Default
	{
		get
		{
			if (_default == null)
			{
				lock (_defaultLock)
				{
					if (_default == null)
					{
						_default = GetMessengerByStorageType();
					}
				}
			}
			return _default;
		}
	}
	private static Messenger _default;
	private static readonly object _defaultLock = new object();

	/// <summary>
	/// Typ úložiště messengera (Session, Cookies).
	/// </summary>
	public static MessengerStorageType StorageType
	{
		get
		{
			return _messengerStorageType;
		}
		set
		{
			lock (_defaultLock)
			{
				if (_default != null)
				{
					throw new InvalidOperationException("Messenger.StorageType lze nastavit pouze před prvním přístupem k Messenger.Default.");
				}
				_messengerStorageType = value;
			}
		}

	}
	private static MessengerStorageType _messengerStorageType = MessengerStorageType.Session;

	private static Messenger GetMessengerByStorageType()
	{
		switch (_messengerStorageType)
		{
			case MessengerStorageType.Session:
				return new SessionMessenger();
			case MessengerStorageType.Cookies:
				return new CookieMessenger();
			default:
				throw new InvalidOperationException("Neznamy typ StorageType.");
		}
	}

	/// <summary>
	/// Zprávy k zobrazení.
	/// </summary>
	public abstract List<MessengerMessage> GetMessages();

	/// <summary>
	/// Přidá zprávu do messengeru.
	/// </summary>
	/// <param name="message">zpráva</param>
	public abstract void AddMessage(MessengerMessage message);

	/// <summary>
	/// Přidá zprávu do messengeru.
	/// </summary>
	/// <param name="messageType">typ zprávy (information, error, ...)</param>
	/// <param name="text">text zprávy</param>
	public void AddMessage(MessageType messageType, string text)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(text), nameof(text));

		AddMessage(new MessengerMessage(text, messageType));
	}

	/// <summary>
	/// Přidá zprávu pomocí String.Format();
	/// </summary>
	/// <param name="messageType">typ zprávy (information, error, ...)</param>
	/// <param name="format">formátovací řetězec pro String.Format()</param>
	/// <param name="args">argumenty pro String.Format()</param>
	public void AddMessage(MessageType messageType, string format, params object[] args)
	{
		AddMessage(messageType, String.Format(format, args));
	}

	/// <summary>
	/// Přidá zprávu typu Information pomocí String.Format();
	/// </summary>
	/// <param name="format">formátovací řetězec pro String.Format()</param>
	/// <param name="args">argumenty pro String.Format()</param>
	public void AddMessage(string format, params object[] args)
	{
		AddMessage(MessageType.Information, String.Format(format, args));
	}

	/// <summary>
	/// Prosté přidání zprávy typu Information.
	/// </summary>
	/// <param name="message">zpráva</param>
	public void AddMessage(string message)
	{
		AddMessage(MessageType.Information, message);
	}

	/// <summary>
	/// Přidá zprávu z App_GlobalResources.
	/// </summary>
	/// <param name="messageType">Typ zprávy.</param>
	/// <param name="classKey">název global-resource souboru</param>
	/// <param name="resourceKey">klíč resourcu</param>
	public void AddGlobalResourceMessage(MessageType messageType, string classKey, string resourceKey)
	{
		AddMessage(messageType, (string)HttpContext.GetGlobalResourceObject(classKey, resourceKey));
	}

	/// <summary>
	/// Přidá zprávu z App_GlobalResources typu Information.
	/// </summary>
	/// <param name="classKey">název global-resource souboru</param>
	/// <param name="resourceKey">klíč resourcu</param>
	public void AddGlobalResourceMessage(string classKey, string resourceKey)
	{
		AddGlobalResourceMessage(MessageType.Information, classKey, resourceKey);
	}

	/// <summary>
	/// Vyčistí kolekci zpráv.
	/// </summary>
	public abstract void Clear();
}
