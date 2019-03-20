using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Havit.Services.DirectoryServices.ActiveDirectory
{
	/// <summary>
	/// Exception thrown when user not found.
	/// </summary>
	public class UserNotFoundException : Exception
	{
		/// <summary>
		/// User which was not found.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		public override string Message
		{
			get
			{
				return String.Format("User {0} not found.", this.UserName);
			}
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public UserNotFoundException(string userName)
		{
			this.UserName = userName;
		}
	}
}
