using Havit.Services.DirectoryServices.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.DirectoryServices
{
	[TestClass]
	public class ActiveDirectoryServicesTest
	{
		#region GetUserInfoTest_SearchByDomain
		[TestMethod]
		public void GetUserInfoTest_SearchByDomain()
		{
			ActiveDirectoryServices services = new ActiveDirectoryServices();
			Assert.IsNotNull(services.GetUserInfo("kanda"), "User 'kanda' not found.");
			Assert.IsNotNull(services.GetUserInfo(@"havit\kanda"), @"User 'havit\kanda' not found.");
			Assert.IsNotNull(services.GetUserInfo(@"HAVIT\kanda"), @"User 'HAVIT\kanda' not found.");
			Assert.IsNotNull(services.GetUserInfo(@"HAVIT\KANDA"), @"User 'HAVIT\KANDA' not found.");
		}
		#endregion

		#region GetUserInfoTest_DetailData
		[TestMethod]
		public void GetUserInfoTest_DetailData()
		{
			ActiveDirectoryServices services = new ActiveDirectoryServices();
			UserInfo userInfo = services.GetUserInfo(@"HAVIT\kanda");
			Assert.IsNotNull(userInfo, @"User 'HAVIT\kanda' not found.");
			Assert.IsNotNull(userInfo.DistinguishedName, "DistinguishedName is null.");
			Assert.IsNotNull(userInfo.DisplayName, "DisplayName is null.");
			Assert.IsNotNull(userInfo.EmailAddresses, "EmailAddresses is null.");
			Assert.IsTrue(userInfo.EmailAddresses.Length > 0, "EmailAddresses contains no email.");
			Assert.IsNotNull(userInfo.FirstName, "FirstName is null.");
			Assert.IsNotNull(userInfo.LastName, "LastName is null.");
		}
		#endregion

	}
}
