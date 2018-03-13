using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EFCore
{
    public static class DbUpdateExceptionExtensions
    {
	    /// <summary>
	    /// Formátuje výjimkou do textu.
	    /// Pokud má dbUpdateException InnerException, vrací sloučený dbUpdateException.Message a dbUpdateException.InnerException.Message.
	    /// Jinak jen text výjimky (dbUpdateException.Message).
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
