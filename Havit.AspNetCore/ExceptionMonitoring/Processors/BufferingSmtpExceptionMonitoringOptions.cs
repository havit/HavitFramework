using System;

namespace Havit.AspNetCore.ExceptionMonitoring.Processors;

/// <summary>
/// Konfigurace odesílání výjimek exception monitoringem na email.
/// </summary>
    public class BufferingSmtpExceptionMonitoringOptions : SmtpExceptionMonitoringOptions
{
	/// <summary>
	/// Indikuje, zda je bufferování povoleno (pokud není povoleno, zpracují se okamžitě všechny výjimky.
	/// </summary>
	public bool BufferingEnabled { get; set; } = true;

	/// <summary>
	/// Interval bufferování výjimek (po jakou dobu není "stejná" výjimka znovu zpracována). V sekundách.
	/// </summary>
	public int BufferingInterval { get; set; } = 60;
    }
