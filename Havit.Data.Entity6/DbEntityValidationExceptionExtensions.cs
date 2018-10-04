using System;
using System.Linq;

namespace Havit.Data.Entity
{
	/// <summary>
	/// Extension metody k třídě DbEntityValidationException.
	/// </summary>
	public static class DbEntityValidationExceptionExtensions
	{
		/// <summary>
		/// Formátuje výjimkou do textu.
		/// Pokud jsou obsaženy validační chyby (EntityValidationErrors), vrací text těchto chyb.
		/// Jinak vrací text (Message) výjimky.
		/// </summary>
		public static string FormatErrorMessage(this System.Data.Entity.Validation.DbEntityValidationException validationException)
		{
			if (!validationException.EntityValidationErrors.Any())
			{
				return validationException.Message;
			}
			else
			{
				return String.Join(" ", validationException.EntityValidationErrors.SelectMany(ve => ve.ValidationErrors.Select(item => item.ErrorMessage)));
			}
		}
	}
}
