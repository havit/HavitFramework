using System.Net.Mail;
using Havit.Diagnostics;

namespace Havit.Tests.Diagnostics;

[TestClass]
public class SmtpTraceListenerTests
{
	[TestMethod]
	public void SmtpTraceListener_GetSmtpClient_ReturnsConfiguredSmtpClient()
	{
		// Arrange
		SmtpTraceListener smtpTraceListener = new SmtpTraceListener("smtpserver=fake;smtpport=999;smtpenablessl=true");

		// Act
		SmtpClient smtpClient = smtpTraceListener.GetSmtpClient();

		// Assert
		Assert.AreEqual("fake", smtpClient.Host);
		Assert.AreEqual(999, smtpClient.Port);
		Assert.IsTrue(smtpClient.EnableSsl);
	}

	[TestMethod]
	public void SmtpTraceListener_ConstructorThrowsExceptionForUnknownValue()
	{
		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			new SmtpTraceListener("unknown=fake");
		});
	}

	[TestMethod]
	public void SmtpTraceListener_ConstructorThrowsExceptionWhenSmtpPortIsConfiguredButNoSmtpServerSet()
	{
		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			new SmtpTraceListener("smtpport=999");
		});
	}
}
