using System;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Extension metody k <see cref="DbUpdateException" />.
	/// </summary>
    public static class DbUpdateExceptionExtensions
    {
	    /// <summary>
	    /// Formátuje výjimkou do textu.
	    /// Pokud má <see cref="DbUpdateException" /> InnerException, vrací sloučený <see cref="DbUpdateException" />.Message a <see cref="DbUpdateException" />.InnerException.Message.
	    /// Jinak jen text výjimky (<see cref="DbUpdateException" />.Message).
	    /// </summary>
	    public static string FormatErrorMessage(this DbUpdateException dbUpdateException)
	    {
		    if (dbUpdateException.InnerException != null)
		    {
			    return String.Join(" ", // separator
				    dbUpdateException.Message,				    
				    dbUpdateException.InnerException.Message);
		    }

		    return dbUpdateException.Message;
	    }
    }
}
