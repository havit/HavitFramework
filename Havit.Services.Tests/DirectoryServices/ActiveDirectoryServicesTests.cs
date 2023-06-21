using Havit.Services.DirectoryServices.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.ActiveDirectory;

namespace Havit.Services.Tests.DirectoryServices;

[TestClass]
public class ActiveDirectoryServicesTests
{
	[TestMethod]
	public void ActiveDirectoryServices_GetUserInfo_SearchByDomain()
	{
		VerifyTestEnvironment();

		ActiveDirectoryServices services = new ActiveDirectoryServices();
		Assert.IsNotNull(services.GetUserInfo("kanda"), "User 'kanda' not found.");
		Assert.IsNotNull(services.GetUserInfo(@"havit\kanda"), @"User 'havit\kanda' not found.");
		Assert.IsNotNull(services.GetUserInfo(@"HAVIT\kanda"), @"User 'HAVIT\kanda' not found.");
		Assert.IsNotNull(services.GetUserInfo(@"HAVIT\KANDA"), @"User 'HAVIT\KANDA' not found.");
	}

	[TestMethod]
	public void ActiveDirectoryServices_GetUserInfo_DetailData()
	{
		VerifyTestEnvironment();

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

	private static void VerifyTestEnvironment()
	{
		try
		{
			if (Domain.GetComputerDomain().Name != "HAVIT")
			{
				Assert.Inconclusive("Test can successfully run only on machine in HAVIT on-prem domain.");
			}
		}
		catch (ActiveDirectoryObjectNotFoundException)
		{
			Assert.Inconclusive("Test can successfully run only on machine in HAVIT on-prem domain.");
		}
	}
}
