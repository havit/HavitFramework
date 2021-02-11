using System;

namespace Havit.AspNetCore.ExceptionMonitoring.Processors
{
	/// <summary>
	/// Konfigurace odesílání výjimek exception monitoringem na email.
	/// </summary>
    public class SmtpExceptionMonitoringOptions
    {
		/// <summary>
		/// Indikuje, zda je odesílání povoleno. Pokud není, emaily se neodesílají.
		/// </summary>
        public bool Enabled { get; set; }

		/// <summary>
		/// Předmět emailu (jako prefix textu výjimky).
		/// </summary>
        public string Subject { get; set; }

	    /// <summary>
	    /// Adresáti emailu (odděleni středníkem).
	    /// </summary>
        public string To { get; set; }

	    /// <summary>
	    /// Odesílatel emailu.
	    /// </summary>
        public string From { get; set; }

		/// <summary>
		/// Smtp server pro odeslání emailu.
		/// </summary>
        public string SmtpServer { get; set; }

	    /// <summary>
	    /// Port smtp serveru pro odeslání emailu.
	    /// </summary>
		public int? SmtpPort { get; set; }

	    /// <summary>
	    /// Indikuje, zda se má použít SSL pro komunikaci se smtp serverem pro odeslání emailu.
	    /// </summary>
        public bool UseSsl { get; set; }

		/// <summary>
		/// Autorizační údaj (username) k smtp serveru.
		/// </summary>
        public string SmtpUsername { get; set; }

	    /// <summary>
	    /// Autorizační údaj (heslo) k smtp serveru.
	    /// </summary>
        public string SmtpPassword { get; set; }

		/// <summary>
		/// Indikuje, zda se mají použít autorizační údaje při komunikaci se smtp serverem. Vrací true, pokud je nastaveno SmtpUsername.
		/// </summary>
        public bool HasCredentials()
        {
            return !String.IsNullOrEmpty(SmtpUsername);
        }
    }
}
