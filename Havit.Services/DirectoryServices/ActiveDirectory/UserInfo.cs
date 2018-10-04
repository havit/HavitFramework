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
		#region DistinguishedName
		/// <summary>
		/// Distinguished name.
		/// </summary>
		/// <example>
		/// CN=Kanda\, Jiří,OU=HAVIT,DC=havit,DC=local
		/// </example>
		public string DistinguishedName { get; internal set; }
		#endregion

		#region DisplayName
		/// <summary>
		/// Display name.
		/// </summary>
		/// <example>
		/// Kanda, Jiří
		/// </example>
		public string DisplayName { get; internal set; }
		#endregion

		#region EmailAddresses
		/// <summary>
		/// Email addresses.
		/// </summary>
		public string[] EmailAddresses { get; internal set; }
		#endregion

		#region AccountName
		/// <summary>
		/// Account name.
		/// </summary>
		/// <example>
		/// HAVIT\kanda
		/// </example>
		public string AccountName { get; internal set; }
		#endregion

		#region FirstName
		/// <summary>
		/// First name (givenName).
		/// </summary>
		public string FirstName { get; internal set; }
		#endregion

		#region LastName
		/// <summary>
		/// Last name (sn).
		/// </summary>
		public string LastName { get; internal set; }
		#endregion

		#region Mobile
		/// <summary>
		/// Mobile number.
		/// </summary>
		public string Mobile { get; internal set; }
		#endregion

		#region Phone
		/// <summary>
		/// Phone number (telephoneNumber).
		/// </summary>
		public string Phone { get; internal set; }
		#endregion
	}
}
