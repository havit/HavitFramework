#if NET46
using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Třída je dostupná pouze pro full .NET Framework (nikoliv pro .NET Standard 2.0).
	/// </summary>
	public class ActiveDirectoryServices
	{
		#region Private fields
		private readonly string domainController;
		private readonly string directoryServicesUsername;
		private readonly string directoryServicesPassword;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates an instance of ActiveDirectoryServices class.
		/// </summary>
		public ActiveDirectoryServices() : this(null, null, null)
		{
		}

		/// <summary>
		/// Creates an instance of ActiveDirectoryServices class.
		/// </summary>
		public ActiveDirectoryServices(string directoryServicesUsername, string directoryServicesPassword)
		{
			this.directoryServicesUsername = directoryServicesUsername;
			this.directoryServicesPassword = directoryServicesPassword;
		}
		
		/// <summary>
		/// Creates an instance of ActiveDirectoryServices class.
		/// </summary>
		/// <param name="directoryServicesUsername">Username for accessing directory services. When empty or null, username and password is not used for accesing directory services.</param>
		/// <param name="directoryServicesPassword">Password for accessing directory services.</param>
		/// <param name="domainController">Specifies which domain controller to use. When not empty, usage of this service is limited to this domain controller domain only. When empty or null, current Domain Controller will be used.</param>
		public ActiveDirectoryServices(string directoryServicesUsername, string directoryServicesPassword, string domainController)
		{
			this.directoryServicesUsername = directoryServicesUsername;
			this.directoryServicesPassword = directoryServicesPassword;
			this.domainController = domainController;
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
		/// <remarks>
		/// Results are "cached" in instance memory, repetitive calls returns same results in zero time.
		/// </remarks>
		public string[] GetGroupMembers(string groupname, bool includeGroups = false, bool traverseNestedGroups = false)
		{
			Contract.Requires(!String.IsNullOrEmpty(groupname));

			// Let's look to the previous calls
			object cacheKey = new { GroupName = groupname.ToLower(), IncludeGroups = includeGroups, TraverseNestedGroups = traverseNestedGroups };
			List<string> members;
			if (_getGroupsMembersCache.TryGetValue(cacheKey, out members))
			{
				return members.ToArray();
			}

			members = new List<string>();
			List<string> processedGroups = new List<string>();
			GetGroupMembersInternal(groupname, includeGroups, traverseNestedGroups, members, processedGroups);
			_getGroupsMembersCache.Add(cacheKey, members);

			return members.ToArray();
		}
		private readonly Dictionary<object, List<string>> _getGroupsMembersCache = new Dictionary<object, List<string>>();

		/// <summary>
		/// Internal method for retrieving group members (used mainly for traversal groups).
		/// </summary>
		private void GetGroupMembersInternal(string groupname, bool includeGroups, bool traverseNestedGroups, List<string> members, List<string> processedGroups)
		{
			// checks a) repeated group by nesting, b) group cycles
			if (processedGroups.Contains(groupname, StringComparer.CurrentCultureIgnoreCase))
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
				throw new GroupNotFoundException(groupname);
			}

			List<SearchResult> searchResults = new List<SearchResult>();

			ResultPropertyValueCollection groupMembersIdentifiers = searchResult.Properties[ActiveDirectoryProperties.Member];
			if (groupMembersIdentifiers.Count > 0)
			{
				string distinguishedNames = String.Join("", groupMembersIdentifiers.OfType<string>().Select(memberIdentifier => string.Format("(distinguishedName={0})", memberIdentifier)));
				
				using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
				{
					searcher.Filter = String.Format("(|{0})", distinguishedNames);
					searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectSid);
					searcher.SizeLimit = groupMembersIdentifiers.Count;
					searchResults.AddRange(searcher.FindAll().Cast<SearchResult>());
				}				
			}

			SecurityIdentifier groupSecurityIdentifier = (SecurityIdentifier)(new NTAccount(domainName, accountName).Translate(typeof(SecurityIdentifier)));
			string groupSsdl = groupSecurityIdentifier.Value;
			string groupPrimaryID = groupSsdl.Substring(groupSsdl.LastIndexOf('-') + 1);

			using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
			{
				searcher.Filter = String.Format("(primaryGroupID={0})", groupPrimaryID);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectSid);
				searchResults.AddRange(searcher.FindAll().Cast<SearchResult>());
			}

			foreach (SearchResult memberSearchResult in searchResults)
			{
				string memberAccountName;
				byte[] sid = (byte[])memberSearchResult.Properties[ActiveDirectoryProperties.ObjectSid][0];
				if (!TryGetAccountName(sid, out memberAccountName))
				{
					continue;
				}

				bool isUser;
				bool isGroup;
				if (!TryGetAccountClass(memberAccountName, out isUser, out isGroup))
				{
					continue;
				}

				if (isUser || (includeGroups && isGroup))
				{
					if (!members.Contains(memberAccountName, StringComparer.CurrentCultureIgnoreCase))
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
		/// Does not support BUILTIN\\... groups.
		/// DOES NOT SUPPORT MULTIDOMAIN ENVIRONMENT - Only groups in the same domain as user are in the result.
		/// </summary>
		/// <param name="username">Username (supported both forms DOMAIN\user and user).</param>
		/// <remarks>
		/// Results are "cached" in instance memory, repetitive calls returns same results in zero time.
		/// </remarks>
		public string[] GetUserDomainMembership(string username)
		{
			Contract.Requires(!String.IsNullOrEmpty(username));
			
			List<string> result;

			string cacheKey = username.ToLower();
			if (_getGroupsMembersCache.TryGetValue(cacheKey, out result))
			{
				return result.ToArray();
			}

			string domainName;
			string accountName;
			SplitNameToDomainAndAccountName(username, out domainName, out accountName);

			SearchResult searchResult;
			using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
			{
				searcher.Filter = String.Format("(&(objectClass=user)(samaccountname={0}))", accountName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectSid);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.MemberOf);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.PrimaryGroupID);
				searchResult = searcher.FindOne();
			}

			if (searchResult == null)
			{
				throw new UserNotFoundException(username);
			}

			result = new List<string>();
			ResultPropertyValueCollection groupIdentifiers = searchResult.Properties[ActiveDirectoryProperties.MemberOf];
			byte[] userSid = (byte[])searchResult.Properties[ActiveDirectoryProperties.ObjectSid][0];
			int? userPrimaryGroupID = searchResult.Properties[ActiveDirectoryProperties.PrimaryGroupID] != null ? (int?)searchResult.Properties[ActiveDirectoryProperties.PrimaryGroupID][0] : null;

			if (groupIdentifiers.Count > 0)
			{
				string distinguishedNames = String.Join("", groupIdentifiers.OfType<string>().Select(memberIdentifier => string.Format("(distinguishedName={0})", memberIdentifier)));

				SearchResultCollection groupSearchResults;
				using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
				{
					searcher.Filter = String.Format("(|{0})", distinguishedNames);
					searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectSid);
					searcher.SizeLimit = groupIdentifiers.Count;
					groupSearchResults = searcher.FindAll();
				}

				foreach (SearchResult groupSearchResult in groupSearchResults)
				{
					string groupName;
					byte[] groupSid = (byte[])groupSearchResult.Properties[ActiveDirectoryProperties.ObjectSid][0];
					if (!TryGetAccountName(groupSid, out groupName))
					{
						continue;
					}

					bool isUser;
					bool isGroup;
					if (!this.TryGetAccountClass(groupName, out isUser, out isGroup))
					{
						continue;
					}

					if (isGroup)
					{
						result.Add(groupName);
					}
				}
			}

			// vyhledání dle primaryGroupID
			if (userPrimaryGroupID != null)
			{
				string primaryGroup = GetPrimaryGroupForSid(domainName, userSid, userPrimaryGroupID.Value);
				if (primaryGroup != null)
				{
					result.Add(primaryGroup);
				}
			}
			
			_getGroupsMembersCache.Add(cacheKey, result);

			return result.ToArray();
		}
		private readonly Dictionary<string, List<string>> _getUserDomainMembershipCache = new Dictionary<string, List<string>>();
		#endregion

		#region GetUserCrossDomainMembership
		/// <summary>
		/// Returns groups from parameter of which user is a member.
		/// Support multidomain envinronment.
		/// </summary>
		/// <param name="username">Username (supported only form DOMAIN\user).</param>
		/// <param name="groups">Groups for which membership is checked.</param>
		/// <param name="traverseNestedGroups">When true, return groups from list when user is member of nested group. Otherwise group is in result only when user is a direct member.</param>
		/// <remarks>
		/// Results are "cached" in instance memory, repetitive calls returns same results in zero time.
		/// </remarks>
		public string[] GetUserCrossDomainMembership(string username, string[] groups, bool traverseNestedGroups = false)
		{
			// "caching" delegated to GetGroupMembers method

			List<string> result = new List<string>();

			foreach (string group in groups)
			{
				List<string> groupMembers = new List<string>(GetGroupMembers(group, false, traverseNestedGroups));
				if (groupMembers.Contains(username, StringComparer.CurrentCultureIgnoreCase))
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
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.DistinguishedName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.DisplayName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.EmailAddress);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectSid);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.FirstName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.LastName);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.Phone);
				searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.Mobile);
				searchResult = searcher.FindOne();
			}

			if (searchResult == null)
			{
				throw new UserNotFoundException(username);
			}

			UserInfo userInfo = new UserInfo();

			string accountNameTmp;
			byte[] sid = (byte[])searchResult.Properties[ActiveDirectoryProperties.ObjectSid][0];
			if (this.TryGetAccountName(sid, out accountNameTmp))
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
			
			if (searchResult.Properties.Contains(ActiveDirectoryProperties.FirstName))
			{
				userInfo.FirstName = searchResult.Properties[ActiveDirectoryProperties.FirstName][0].ToString();
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.LastName))
			{
				userInfo.LastName = searchResult.Properties[ActiveDirectoryProperties.LastName][0].ToString();
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.Phone))
			{
				userInfo.Phone = searchResult.Properties[ActiveDirectoryProperties.Phone][0].ToString();
			}

			if (searchResult.Properties.Contains(ActiveDirectoryProperties.Mobile))
			{
				userInfo.Mobile = searchResult.Properties[ActiveDirectoryProperties.Mobile][0].ToString();
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

			if (!String.IsNullOrEmpty(domainController))
			{				
				directoryContext = String.IsNullOrEmpty(directoryServicesUsername) ? new DirectoryContext(DirectoryContextType.DirectoryServer, domainController) : new DirectoryContext(DirectoryContextType.DirectoryServer, domainController, directoryServicesUsername, directoryServicesPassword);
			}
			else if (String.IsNullOrEmpty(domainName))
			{
				directoryContext = String.IsNullOrEmpty(directoryServicesUsername) ? new DirectoryContext(DirectoryContextType.Domain) : new DirectoryContext(DirectoryContextType.Domain, directoryServicesUsername, directoryServicesPassword);
			}
			else
			{
				directoryContext = String.IsNullOrEmpty(directoryServicesUsername) ? new DirectoryContext(DirectoryContextType.Domain, domainName) : new DirectoryContext(DirectoryContextType.Domain, domainName, directoryServicesUsername, directoryServicesPassword);
			}

			using (Domain domain = Domain.GetDomain(directoryContext))
			{				
				if (!String.IsNullOrEmpty(domainController) && !String.IsNullOrEmpty(domainName))
				{
					// kontrola domény v situaci, kdy je zadán domain controller a je požadována konkrétní doména
					// test je značně nedokonalý, porovnáváme jen textové hodnoty
					
					if ((domainName.Contains(".") && !String.Equals(domain.Name, domainName, StringComparison.CurrentCultureIgnoreCase)) // pokud je v požadované doméně tečka, jde o celý název domény
						|| (!domainName.Contains(".") && !domain.Name.StartsWith(domainName + ".", StringComparison.CurrentCultureIgnoreCase))) // pokud není v požadované doméně tečka, nesprávně tvrdíme, že tímto textem musí skutečný název domény začínat
					{
						throw new InvalidOperationException(String.Format("Domain controller '{0}' obsluhuje doménu '{1}', nikoliv požadovanou doménu '{2}'.", domainController, domain.Name, domainName));
					}
				}

				using (DirectoryEntry directoryEntry = domain.GetDirectoryEntry())
				{								
					DirectorySearcher searcher = new DirectorySearcher(directoryEntry);
					return searcher;
				}
			}
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
		private bool TryGetAccountName(byte[] sid, out string accountName)
		{
			try
			{
				SecurityIdentifier securityIdentifier = new SecurityIdentifier(sid, 0);				
				accountName = ((NTAccount)securityIdentifier.Translate(typeof(NTAccount))).ToString();				
				if (accountName.StartsWith("BUILTIN\\")) // Pro buildin
				{
					Trace.Write(String.Format("Skipped accountname {0} due internal implementation.", accountName), typeof(ActiveDirectoryServices).Name);
					return false;
				}
				return true;
			}
			catch
			{
				accountName = null;
				return false;
			}
		}
		#endregion

		#region TryGetAccountClass
		/// <summary>
		/// Returns account class (user, group).
		/// </summary>
		/// <remarks>
		/// Results are "cached" in instance memory, repetitive calls returns same results in zero time.
		/// </remarks>
		private bool TryGetAccountClass(string name, out bool isUser, out bool isGroup)
		{
			string cacheKey = name.ToLower();
			TryGetAccountClass_Data resultData;
			if (!_tryGetAccountClass.TryGetValue(cacheKey, out resultData))
			{
				string domainName;
				string accountName;
				SplitNameToDomainAndAccountName(name, out domainName, out accountName);

				SearchResult searchResult;
				using (DirectorySearcher searcher = GetDirectorySearcher(domainName))
				{
					searcher.Filter = string.Format("(samaccountname={0})", accountName);
					searcher.PropertiesToLoad.Add(ActiveDirectoryProperties.ObjectClass);
					searchResult = searcher.FindOne();
				}

				if (searchResult == null)
				{
					resultData = new TryGetAccountClass_Data
					{
						IsUser = false,
						IsGroup = false,
						Result = false
					};
				}
				else
				{
					resultData = new TryGetAccountClass_Data
					{
						IsUser = searchResult.Properties[ActiveDirectoryProperties.ObjectClass].Contains("user"),
						IsGroup = searchResult.Properties[ActiveDirectoryProperties.ObjectClass].Contains("group"),
						Result = true
					};
				}
				_tryGetAccountClass.Add(cacheKey, resultData);
			}

			isUser = resultData.IsUser;
			isGroup = resultData.IsGroup;
			return resultData.Result;
		}
		private readonly Dictionary<string, TryGetAccountClass_Data> _tryGetAccountClass = new Dictionary<string, TryGetAccountClass_Data>();
		private class TryGetAccountClass_Data
		{
			public bool IsUser { get; set; }
			public bool IsGroup { get; set; }
			public bool Result { get; set; }
		}
		#endregion

		#region GetPrimaryGroupForSid
		/// <summary>
		/// Returns primary group for user.
		/// </summary>
		private string GetPrimaryGroupForSid(string domainName, byte[] userSid, int userPrimaryGroupID)
		{
			// http://support.microsoft.com/kb/297951/en-us
			SecurityIdentifier userSecurityIdentifier = new SecurityIdentifier(userSid, 0);
			string userSsdl = userSecurityIdentifier.Value;
			string primaryGroupSsdl = userSsdl.Left(userSsdl.LastIndexOf('-')) + "-" + userPrimaryGroupID.ToString();

			SecurityIdentifier primaryGroupSecurityIdentifier = new SecurityIdentifier(primaryGroupSsdl);
			byte[] primaryGroupSid = new byte[primaryGroupSecurityIdentifier.BinaryLength];
			primaryGroupSecurityIdentifier.GetBinaryForm(primaryGroupSid, 0);
			
			string result;
			if (this.TryGetAccountName(primaryGroupSid, out result))
			{
				return result;
			}			
			return null;
		}
		#endregion

	}
}
#endif