using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Předek výjimek vracených z ARESu.
	/// </summary>	
	public abstract class AresBaseException : ApplicationException
	{
		#region Construktor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Exception message.</param>
		[SuppressMessage("SonarLint", "S3442", Justification = "Definovat použitelného potomka chceme jen v této assembly, proto třídu ochraňujeme internal contructory.")]
		internal AresBaseException(string message)
			: base(message)
		{

		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AresBaseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        [SuppressMessage("SonarLint", "S3442", Justification = "Definovat použitelného potomka chceme jen v této assembly, proto třídu ochraňujeme internal contructory.")]
		internal AresBaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}
