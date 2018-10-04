using Havit.AspNet.Mvc.Messenger;
using Havit.AspNet.Mvc.Messenger.Storages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;

namespace Havit.AspNet.Mvc.Tests.Messenger
{
	[TestClass]
	public class ToastrMessengerTests
	{
		[TestMethod]
		public void ToastrMessenger_AddMessages()
		{
			Mock<HttpSessionStateBase> httpSessionState = new Mock<HttpSessionStateBase>();

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(x => x.Session).Returns(httpSessionState.Object);

			Mock<IMessageStorage> messageStorage = new Mock<IMessageStorage>();
			messageStorage.Setup(x => x.AddMessage(It.IsAny<Message>()));

			IMessenger messenger = new ToastrMessenger(messageStorage.Object);
			messenger.AddMessage("test");

			messageStorage.Verify(x => x.AddMessage(It.IsAny<Message>()), Times.Once());
		}
	}
}
