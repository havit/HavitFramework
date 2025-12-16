using Havit.AspNetCore.ExceptionMonitoring.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Havit.AspNetCore.ExceptionMonitoring.Processors;

/// <summary>
/// Exception procesor zasílající výjimku na email.
/// Zajiští, aby nedošlo k zaslání stejné chyby opakovaně (v časovém okně).
/// </summary>
public class BufferingSmtpExceptionMonitoringProcessor : SmtpExceptionMonitoringProcessor
{
	private readonly ILogger<BufferingSmtpExceptionMonitoringProcessor> _logger;
	private readonly BufferingSmtpExceptionMonitoringOptions _options;
	private readonly IMemoryCache _memoryCache;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public BufferingSmtpExceptionMonitoringProcessor(IExceptionFormatter exceptionFormatter, IOptions<BufferingSmtpExceptionMonitoringOptions> options, ILogger<BufferingSmtpExceptionMonitoringProcessor> logger, IMemoryCache memoryCache) : base(exceptionFormatter, options, logger)
	{
		this._logger = logger;
		this._memoryCache = memoryCache;
		this._options = options.Value;
	}

	/// <summary>
	/// Zpravuje výjimku zaslanou do exception monitoringu.
	/// Odesílá výjimku na email.
	/// </summary>
	protected override void ProcessExceptionCore(Exception exception)
	{
		if (ShouldProcessException(exception))
		{
			base.ProcessExceptionCore(exception);
		}
	}

	/// <summary>
	/// Vrací true, pokud má být výjimka zpracována (odeslána). Odeslání může být dále zastaveno vypnutím zasílání zpráv.
	/// Identifikuje, zda v daném období již stejná výjimka byla vyhozena. Porovnání výjimek je uděláno porovnáním typu výjimky a textu stack trace.
	/// Text (message) výjimky není do porovnání použit.
	/// </summary>
	protected internal virtual bool ShouldProcessException(Exception exception)
	{
		if (!_options.BufferingEnabled)
		{
			_logger.LogTrace("Buffering disabled.");
			return true;
		}

		object exceptionKey = GetExceptionKey(exception);
		BufferCounter bufferCounter = _memoryCache.GetOrCreate(exceptionKey, (cacheEntry) =>
		{
			_logger.LogTrace("Adding item to buffer.");
			cacheEntry.SetPriority(CacheItemPriority.NeverRemove).SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.BufferingInterval));
			return new BufferCounter();
		});

		bool shouldProcessException;
		lock (bufferCounter)
		{
			shouldProcessException = bufferCounter.Counter == 0;
			bufferCounter.Counter += 1;
		}

		if (!shouldProcessException)
		{
			_logger.LogTrace("Exception should not be processed (it is currently in the buffer).");
		}

		return shouldProcessException;
	}

	/// <summary>
	/// Returns key to identify duplicates of the exception.
	/// Must be comparable (record, Equals and == operator implementation, etc.)
	/// </summary>
	protected virtual object GetExceptionKey(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception);
		return new ExceptionInfo
		{
			ExceptionType = exception.GetType(),
			StackTrace = exception.StackTrace
		};
	}

	internal record ExceptionInfo
	{
		public Type ExceptionType { get; set; }
		public string StackTrace { get; set; }
	}

	private class BufferCounter
	{
		public int Counter { get; set; }
	}

}