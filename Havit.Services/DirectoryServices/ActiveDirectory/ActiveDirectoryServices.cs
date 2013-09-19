using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.DirectoryServices.ActiveDirectory
{
	/// <summary>
	/// Active directory services.
	/// </summary>
	public class ActiveDirectoryServices
	{
		#region Private fields
		private string directoryServicesUsername;
		private string directoryServicesPassword;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates an instance of ActiveDirectoryServices class.
		/// </summary>
		public ActiveDirectoryServices() : this(null, null)
		{
		}

		/// <summary>
		/// Creates an instance of ActiveDirectoryServices class.
		/// </summary>
		/// <param name="directoryServicesUsername">Username for accessing directory services. When empty or null, username and password is not used for accesing directory services.</param>
		/// <param name="directoryServicesPassword">Password for accessing directory services.</param>
		public ActiveDirectoryServices(string directoryServicesUsername, string directoryServicesPassword)
		{
			this.directoryServicesUsername = directoryServicesUsername;
			this.directoryServicesPassword = directoryServicesPassword;
		}
		#endregion

		#region GetGroupMembers, GetGroupMembersInternal
		/// <summary>
		/// Returns group members.
		/// Supports multi-domain environment.
		/// </summary>
		/// <param name="groupname">Group name (supported both forms DOMAIN\group and group).</param>
		/// <param name="includeGroups">When true, result contains members of type group (otherwise contains users only).</param>
		/// <param name="traverseNestedGroups">When true, result includes members of nested groups (members of members).</param>
		/// <exception cref="System.InvalidOperationException">Group not found.</exception>
		public string[] GetGroupMembers(string groupname, bool includeGroups = false, bool traverseNestedGroups = false)
		{
			Contract.Requires(!String.IsNullOrEmpty(groupname));

			List<string> members = new List<string>();
			List<string> processedGroups = new List<string>();
			GetGroupMembersInternal(groupname, includeGroups, traverseNestedGroups, members, processedGroups);
			return members.ToArray();
		}

		/// <summary>
		/// Internal method for retrieving group members (used mainly for traversal groups).
		/// </summary>
		private void GetGroupMembersInternal(string groupname, bool includeGroups, bool traverseNestedGroups, List<string> members, List<string> processedGroups)
		{
			// checks a) repeated group by nesting, b) group cycles
			if (processedGroups.Contains(groupname))
			{
				return;
			}
			processedGroups.Add(groupname);

			string domainName;
			string accountName;
			SplitNameToDomainAndAccountName(groupname, out domainName, out accountName);

			SearchResult searchResult;
			using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
			{
				searcher.Filter = String.Format("(&(objectClass=group)(samaccountname={0}))", accountName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.Member);
				searchResult = searcher.FindOne();
			}
			
			if (searchResult == null)
			{
				throw new InvalidOperationException("Group not found.");
			}

			ResultPropertyValueCollection groupMembersIdentifiers = searchResult.Properties[ActiveDirectoryProperties.Member];
			foreach (object memberIdentifier in groupMembersIdentifiers)
			{
				DirectoryEntry user = new DirectoryEntry("LDAP://" + memberIdentifier.ToString());

				object[] objectClasses = (object[])user.InvokeGet(ActiveDirectoryProperties.ObjectClass);
				bool isUser = objectClasses.Contains("user"); // todo constants
				bool isGroup = objectClasses.Contains("group"); // todo constants

				if (!isUser && !isGroup)
				{
					continue;
				}

				string memberAccountName;
				if (!TryGetAccountName(user, out memberAccountName))
				{
					continue;
				}

				if (isUser || (includeGroups && isGroup))
				{
					if (!members.Contains(memberAccountName))
					{
						members.Add(memberAccountName);
					}
				}

				if (isGroup && traverseNestedGroups)
				{
					GetGroupMembersInternal(memberAccountName, includeGroups, traverseNestedGroups /* true */, members, processedGroups);
				}
			}
		}
		#endregion

		#region GetUserMembership
		/// <summary>
		/// Returns users membership (groups of which is a member).
		/// DOES NOT SUPPORT MULTIDOMAIN ENVIRONMENT - Only groups in the same domain as user are in the result.
		/// </summary>
		/// <param name="username">Username (supported both forms DOMAIN\user and user).</param>
		public string[] GetUserDomainMembership(string username)
		{
			string domainName;
			string accountName;
			SplitNameToDomainAndAccountName(username, out domainName, out accountName);

			SearchResult searchResult;
			using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
			{
				searcher.Filter = String.Format("(&(objectClass=user)(samaccountname={0}))", accountName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.MemberOf);
				searchResult = searcher.FindOne();
			}

			if (searchResult == null)
			{
				throw new InvalidOperationException("User not found.");
			}

			List<string> result = new List<string>();

			ResultPropertyValueCollection groupIdentifiers = searchResult.Properties[ActiveDirectoryProperties.MemberOf];
			foreach (object groupIdentifier in groupIdentifiers)
			{
				DirectoryEntry group = new DirectoryEntry("LDAP://" + groupIdentifier.ToString());

				object[] objectClasses = (object[])group.InvokeGet(ActiveDirectoryProperties.ObjectClass);
				if (!objectClasses.Contains("group"))
				{
					continue;
				}

				string groupName;
				if (!TryGetAccountName(group, out groupName))
				{
					continue;
				}

				result.Add(groupName);
			}

			return result.ToArray();
		}
		#endregion

		#region GetUserCrossDomainMembership
		/// <summary>
		/// Returns groups from parameter of which user is a member.
		/// Support multidomain envinronment.
		/// </summary>
		/// <param name="username">Username (supported both forms DOMAIN\user and user).</param>
		/// <param name="groups">Groups for which membership is checked.</param>
		/// <param name="traverseNestedGroups">When true, return groups from list when user is member of nested group. Otherwise group is in result only when user is a direct member.</param>
		public string[] GetUserCrossDomainMembership(string username, string[] groups, bool traverseNestedGroups = false)
		{
			List<string> result = new List<string>();

			foreach (string group in groups)
			{
				List<string> groupMembers = new List<string>(GetGroupMembers(group, false, traverseNestedGroups));
				if (groupMembers.Contains(username))
				{
					result.Add(group);
				}
			}

			return result.ToArray();
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Return user info.
		/// </summary>
		/// <param name="username">Username (supported both forms DOMAIN\user and user).</param>
		public UserInfo GetUserInfo(string username)
		{
			string domainName;
			string accountName;
			SplitNameToDomainAndAccountName(username, out domainName, out accountName);

			SearchResult searchResult;
			using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
			{
				searcher.Filter = string.Format("(&(objectClass=user)(samaccountname={0}))", accountName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.DisplayName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.EmailAddress);

				searcher.PageSize = 1000;
				searcher.SizeLimit = 0;

				searchResult = searcher.FindOne();
			}

			if (searchResult == null)
			{
				throw new InvalidOperationException("User not found.");
			}

			UserInfo userInfo = new UserInfo();

			string accountNameTmp;
			if (this.TryGetAccountName(searchResult.GetDirectoryEntry(), out accountNameTmp))
			{
				userInfo.AccountName = accountNameTmp;
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.EmailAddress))
			{
				userInfo.EmailAddresses = searchResult.Properties[ActiveDirectoryProperties.EmailAddress].OfType<String>().ToArray();
			}
			else
			{
				userInfo.EmailAddresses = new string[0];
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.DistinguishedName))
			{
				userInfo.DistinguishedName = searchResult.Properties[ActiveDirectoryProperties.DistinguishedName][0].ToString();
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.DisplayName))
			{
				userInfo.DisplayName = searchResult.Properties[ActiveDirectoryProperties.DisplayName][0].ToString();
			}

			return userInfo;
		}
		#endregion

		#region GetDirectorySearcher
		/// <summary>
		/// Returns directory searcher for given domain name.
		/// </summary>
		private DirectorySearcher GetDirectorySearcher(string domainName)
		{
			DirectoryContext directoryContext;
			if (String.IsNullOrEmpty(domainName))
			{
				directoryContext = String.IsNullOrEmpty(directoryServicesUsername) ? new DirectoryContext(DirectoryContextType.Domain) : new DirectoryContext(DirectoryContextType.Domain, directoryServicesUsername, directoryServicesPassword);
			}
			else
			{
				directoryContext = String.IsNullOrEmpty(directoryServicesUsername) ? new DirectoryContext(DirectoryContextType.Domain, domainName) : new DirectoryContext(DirectoryContextType.Domain, domainName, directoryServicesUsername, directoryServicesPassword);
			}

			Domain domain = Domain.GetDomain(directoryContext);
			DirectoryEntry directoryEntry = domain.GetDirectoryEntry();
			DirectorySearcher searcher = new DirectorySearcher(directoryEntry);

			return searcher;
		}
		#endregion

		#region SplitNameToDomainAndAccountName
		/// <summary>
		/// Splits name to domain name and account name.
		/// </summary>
		private void SplitNameToDomainAndAccountName(string name, out string domainName, out string accountName)
		{
			Contract.Requires(!String.IsNullOrEmpty(name));

			string[] nameParts = name.Split('\\');
			if (nameParts.Length == 1)
			{
				domainName = null;
				accountName = nameParts[0];
			}
			else
			{
				domainName = nameParts[0];
				accountName = nameParts[1];
			}
		}
		#endregion

		#region TryGetAccountName
		/// <summary>
		/// Retrieves object sid and translates it to name (using NTAccount class) in HAVIT\\everyone format.
		/// When translation succedes returns true, otherwise false.
		/// </summary>
		private bool TryGetAccountName(DirectoryEntry objectEntry, out string accountName)
		{
			object objectSid = objectEntry.InvokeGet("objectSid");

			try
			{
				SecurityIdentifier sid = new SecurityIdentifier((byte[])objectSid, 0);
				accountName = ((NTAccount)sid.Translate(typeof(NTAccount))).ToString();
				return true;
			}
			catch
			{
				accountName = null;
				return false;
			}
		}
		#endregion

	}
}
