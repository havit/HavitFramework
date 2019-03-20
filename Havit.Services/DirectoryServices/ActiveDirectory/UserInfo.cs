using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.DirectoryServices.ActiveDirectory
{
	/// <summary>
	/// User info DTO.
	/// </summary>
	public class UserInfo
	{
		/// <summary>
		/// Distinguished name.
		/// </summary>
		/// <example>
		/// CN=Kanda\, Jiří,OU=HAVIT,DC=havit,DC=local
		/// </example>
		public string DistinguishedName { get; internal set; }

		/// <summary>
		/// Display name.
		/// </summary>
		/// <example>
		/// Kanda, Jiří
		/// </example>
		public string DisplayName { get; internal set; }

		/// <summary>
		/// Email addresses.
		/// </summary>
		public string[] EmailAddresses { get; internal set; }

		/// <summary>
		/// Account name.
		/// </summary>
		/// <example>
		/// HAVIT\kanda
		/// </example>
		public string AccountName { get; internal set; }

		/// <summary>
		/// First name (givenName).
		/// </summary>
		public string FirstName { get; internal set; }

		/// <summary>
		/// Last name (sn).
		/// </summary>
		public string LastName { get; internal set; }

		/// <summary>
		/// Mobile number.
		/// </summary>
		public string Mobile { get; internal set; }

		/// <summary>
		/// Phone number (telephoneNumber).
		/// </summary>
		public string Phone { get; internal set; }
	}
}
