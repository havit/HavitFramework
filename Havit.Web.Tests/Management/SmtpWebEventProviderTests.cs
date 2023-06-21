using Havit.Web.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Tests.Management;

[TestClass]
public class SmtpWebEventProviderTests
{
	[TestMethod]
	public void SmtpWebEventProvider_Initialize_NoConfiguration()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection();
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - no exception is thrown
	}

	[TestMethod]
	public void SmtpWebEventProvider_Initialize_From()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "from", "info@havit.cz" }
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - no exception is thrown
	}

	[TestMethod]
	public void SmtpWebEventProvider_Initialize_FromAndSmtpServer()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "from", "info@havit.cz" },
			{ "smtpServer", "mail.havit.local" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - no exception is thrown
	}

	[TestMethod]
	public void SmtpWebEventProvider_Initialize_FromAndAllSmtpSetting()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "from", "info@havit.cz" },
			{ "smtpServer", "mail.havit.local" },
			{ "smtpUsername", "username" },
			{ "smtpPassword", "password" },
			{ "smtpPort", "25" },
			{ "smtpEnableSsl", "true" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - no exception is thrown
	}

	[TestMethod]
	[ExpectedException(typeof(ConfigurationErrorsException))]
	public void SmtpWebEventProvider_Initialize_SmtpUsernameWithoutSmtpServer()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "smtpUsername", "username" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - exception by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ConfigurationErrorsException))]
	public void SmtpWebEventProvider_Initialize_SmtpPortWithoutSmtpServer()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "smtpPort", "25" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - exception by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ConfigurationErrorsException))]
	public void SmtpWebEventProvider_Initialize_SmtpEnableSslWithoutSmtpServer()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "smtpEnableSsl", "true" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - exception by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ConfigurationErrorsException))]
	public void SmtpWebEventProvider_Initialize_InvalidValue()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "invalid", "value" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();

		// Act
		smtpWebEventProvider.Initialize("name", configuration);

		// Assert - exception by method attribute
	}

	[TestMethod]
	public void SmtpWebEventProvider_GetSmtpClient_IsInitialized()
	{
		// Arrange
		NameValueCollection configuration = new NameValueCollection()
		{
			{ "smtpServer", "mail.havit.local" },
			{ "smtpUsername", "username" },
			{ "smtpPassword", "password" },
			{ "smtpPort", "587" },
			{ "smtpEnableSsl", "true" },
		};
		SmtpWebEventProvider smtpWebEventProvider = new SmtpWebEventProvider();
		smtpWebEventProvider.Initialize("name", configuration);

		// Act
		SmtpClient smtpClient = smtpWebEventProvider.GetSmtpClient();

		// Assert
		Assert.AreEqual("mail.havit.local", smtpClient.Host);
		Assert.AreEqual("username", ((NetworkCredential)smtpClient.Credentials).UserName);
		Assert.AreEqual("password", ((NetworkCredential)smtpClient.Credentials).Password);
		Assert.AreEqual(587, smtpClient.Port);
		Assert.AreEqual(true, smtpClient.EnableSsl);
		Assert.AreEqual(SmtpDeliveryMethod.Network, smtpClient.DeliveryMethod);
	}

}
