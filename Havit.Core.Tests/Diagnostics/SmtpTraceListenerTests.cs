using Havit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Tests.Diagnostics
{
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
		[ExpectedException(typeof(ArgumentException))]
		public void SmtpTraceListener_ConstructorThrowsExceptionForUnknownValue()
		{
			// Act
			new SmtpTraceListener("smtp_fake=fake");

			// Assert by method attribute			
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SmtpTraceListener_ConstructorThrowsExceptionWhenSmtpPortIsConfiguredButNoSmtpServerSet()
		{
			// Act
			new SmtpTraceListener("smtpport=999");
			
			// Assert by method attribute
		}
	}
}
