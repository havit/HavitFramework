using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using Havit.AspNet.Mvc.Messenger.Storages;
using Havit.AspNet.Mvc.Messenger;

namespace Havit.AspNet.Mvc.Tests.Messenger.Storages
{
	[TestClass]
	public class SessionMessageStorageTests
	{
		[TestMethod]
		public void Messenger_SessionStorage_AddMessage()
		{
			Mock<HttpSessionStateBase> sessionMock = new Mock<HttpSessionStateBase>();

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Session).Returns(sessionMock.Object);

			IMessageStorage messageStorage = new SessionMessageStorage(contextMock.Object);
			messageStorage.AddMessage(new Message { Text = String.Empty, MessageType = MessageType.Success });

			sessionMock.Verify(session => session[SessionMessageStorage.StorageKey], Times.Once());
		}

		[TestMethod]
		public void Messenger_SessionStorage_GetMessages()
		{
			Mock<HttpSessionStateBase> sessionMock = new Mock<HttpSessionStateBase>();

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Session).Returns(sessionMock.Object);

			IMessageStorage messageStorage = new SessionMessageStorage(contextMock.Object);
			messageStorage.GetMessages();

			sessionMock.Verify(session => session[SessionMessageStorage.StorageKey], Times.Once());
		}

		[TestMethod]
		public void Messenger_SessionStorage_ClearMessages()
		{
			Mock<HttpSessionStateBase> sessionMock = new Mock<HttpSessionStateBase>();

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Session).Returns(sessionMock.Object);

			IMessageStorage messageStorage = new SessionMessageStorage(contextMock.Object);
			messageStorage.ClearMessages();

			sessionMock.Verify(session => session[SessionMessageStorage.StorageKey], Times.Once());
		}
	}
}
