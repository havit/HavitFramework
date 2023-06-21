using Havit.AspNetCore.ExceptionMonitoring.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Havit.AspNetCore.ExceptionMonitoring.Processors;

    /// <summary>
    /// Exception procesor zasílající výjimku na email.
    /// </summary>
    public class SmtpExceptionMonitoringProcessor : IExceptionMonitoringProcessor
    {
        private readonly IExceptionFormatter exceptionFormatter;
	private readonly ILogger<SmtpExceptionMonitoringProcessor> logger;
	private readonly SmtpExceptionMonitoringOptions options;

        private int _mailCounter = 0;

	/// <summary>
	/// Konstruktor.
	/// </summary>
        public SmtpExceptionMonitoringProcessor(IExceptionFormatter exceptionFormatter, IOptions<SmtpExceptionMonitoringOptions> options, ILogger<SmtpExceptionMonitoringProcessor> logger)
        {
            this.exceptionFormatter = exceptionFormatter;
		this.logger = logger;
		this.options = options.Value;
        }

	/// <summary>
	/// Zpracuje výjimku zaslanou do exception monitoringu.
	/// Odesílá výjimku na email.
	/// </summary>
        public void ProcessException(Exception exception)
        {
		bool enabled = options.Enabled;

		logger.LogTrace(enabled ? "SmtpExceptionMonitoringProcessor enabled." : "SmtpExceptionMonitoringProcessor disabled.");

		if (enabled)
            {
                ProcessExceptionCore(exception);
            }
        }

        /// <summary>
	/// Zpracuje výjimku zaslanou do exception monitoringu.
	/// Odesílá výjimku na email.
        /// </summary>
        protected virtual void ProcessExceptionCore(Exception exception)
        {
            MailMessage mailMessage = PrepareMailMessage(exception);

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Host = options.SmtpServer;
                if (options.SmtpPort != null)
                {
                    smtpClient.Port = options.SmtpPort.Value;
                }
                smtpClient.EnableSsl = options.UseSsl;
                if (options.HasCredentials())
                {
                    smtpClient.Credentials = new NetworkCredential(options.SmtpUsername, options.SmtpPassword);
                }

                logger.LogTrace("Sending message.");
                smtpClient.Send(mailMessage);
                logger.LogInformation("Message sent.");

            }
        }

        /// <summary>
        /// Vrací mail message k odeslání.
        /// </summary>
        protected virtual MailMessage PrepareMailMessage(Exception exception)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;

            mailMessage.From = GetMailMessageFrom(exception);

            var toAddresses = GetMailMessageTo(exception);
            toAddresses.ForEach(toAddress => mailMessage.To.Add(toAddress));
            mailMessage.Subject = GetMailMessageSubject(exception);
            mailMessage.Body = GetMailMessageBody(exception);

            return mailMessage;
        }

	/// <summary>
	/// Vratí odesílatele emailu.
	/// </summary>
        protected virtual MailAddress GetMailMessageFrom(Exception exception)
        {
            return new MailAddress(options.From);
        }

    /// <summary>
    /// Vratí adresáta emailu.
    /// </summary>
        protected virtual List<MailAddress> GetMailMessageTo(Exception exception)
        {
            List<MailAddress> result = new List<MailAddress>();

            if (!String.IsNullOrEmpty(options.Subject))
            {
                foreach (string recipient in options.To.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(new MailAddress(recipient.Trim()));
                }
            }

            return result;
        }

    /// <summary>
    /// Vratí předmět emailu.
    /// </summary>
        protected virtual string GetMailMessageSubject(Exception exception)
        {
            string message = exception.Message;

            // předmět mailu nesmí obsahovat znaky \r a \n
            if (message.Contains("\r"))
            {
                message = message.Left(message.IndexOf('\r'));
            }
            if (message.Contains("\n"))
            {
                message = message.Left(message.IndexOf('\n'));
            }

            int counter = Interlocked.Increment(ref _mailCounter);

            return $"{options.Subject}: {message} (#{counter})";
        }

	/// <summary>
	/// Vrátí text emailu.
	/// </summary>
        protected virtual string GetMailMessageBody(Exception exception) => exceptionFormatter.FormatException(exception);
    }