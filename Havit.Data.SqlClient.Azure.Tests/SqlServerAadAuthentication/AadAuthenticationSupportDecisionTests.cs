using Havit.Data.SqlClient.Azure.SqlServerAadAuthentication;

namespace Havit.Data.SqlClient.Azure.Tests.SqlServerAadAuthentication;

[TestClass]
public class AadAuthenticationSupportDecisionTests
{
	[TestMethod]
	public void AadAuthenticationSupportDecision_ShouldUseAadAuthentication()
	{
		// Act + Assert

		// no database.windows.net in Data Source
		Assert.IsFalse(AadAuthenticationSupportDecision.ShouldUseAadAuthentication("Data Source=fake;Initial Catalog=fake;User Id=fake;Password=fake"));

		// no database.windows.net in Data Source
		Assert.IsFalse(AadAuthenticationSupportDecision.ShouldUseAadAuthentication("Data Source=fake;Initial Catalog=fake"));

		// no database.windows.net in Data Source
		Assert.IsFalse(AadAuthenticationSupportDecision.ShouldUseAadAuthentication("Data Source=fake;Initial Catalog=fake;Application Name=fake.database.windows.net"));

		// database.windows.net but User Id specified
		Assert.IsFalse(AadAuthenticationSupportDecision.ShouldUseAadAuthentication("Data Source=fake.database.windows.net;Initial Catalog=fake;User Id=fake;Password=fake"));

		// database.windows.net in Data Source and no User Id
		Assert.IsTrue(AadAuthenticationSupportDecision.ShouldUseAadAuthentication("Data Source=fake.database.windows.net;Initial Catalog=fake"));
	}
}
