using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;
using System.Web.Services.Protocols;
using Havit.Web.Management;
using System.Web;

namespace Havit.Web.Services.Extensions;

/// <summary>
/// V případě chyby ve zpracování web metody (webové služby) zajistí oznámení chyby health monitoringem.
/// Pozor, toto nefunguje (a chyby healthmonitoringu tak nejsou oznamovány),
/// pokud se webové služby testují v browseru!!! Pro testování nutno použít skutečného klienta webové služby (třeba service reference v konzolovce).
/// </summary>
public class SoapExceptionHealthMonitoringHandler : SoapExceptionExceptionHandler
{
	/// <summary>
	/// Zpracovává SoapMessage, která obsahuje výjimku.
	/// Volá ShouldRaiseEvent a pokud je vráceno true, volá RaiseEvent.
	/// </summary>
	protected override sealed void ProcessMessageException(SoapMessage message)
	{
		Exception exception = message.Exception.InnerException;
		if (exception != null)
		{
			if (ShouldRaiseEvent(exception))
			{
				RaiseEvent(message, exception);
			}
		}
	}

	/// <summary>
	/// Vrací true, pokud má dojít k vyvolání událost v metodě RaiseEvent.
	/// Implementace vrací vždy true.
	/// </summary>
	protected virtual bool ShouldRaiseEvent(Exception exception)
	{
		return true;
	}

	/// <summary>
	/// Vyvolá událost vytvořením WebRequestErrorEventExt a zavoláním Raise.
	/// </summary>
	protected virtual void RaiseEvent(SoapMessage message, Exception exception)
	{
		new WebRequestErrorEventExt(exception.Message, message, exception, HttpContext.Current).Raise();
	}
}