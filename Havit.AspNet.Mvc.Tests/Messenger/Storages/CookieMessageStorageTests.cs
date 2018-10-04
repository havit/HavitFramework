using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using Havit.AspNet.Mvc.Messenger.Storages;
using Havit.AspNet.Mvc.Messenger;
using System.Collections.Generic;
using System.Linq;

namespace Havit.AspNet.Mvc.Tests.Messenger.Storages
{
	[TestClass]
	public class CookieMessageStorageTests
	{
		[TestMethod]
		public void Messenger_CookieStorage_AddMessage()
		{
			// arrange
			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Request.Cookies).Returns(new HttpCookieCollection());
			contextMock.Setup(context => context.Response.Cookies).Returns(new HttpCookieCollection());
			
			// act
			IMessageStorage messageStorage = new CookieMessageStorage(contextMock.Object);
			messageStorage.AddMessage(new Message { Text = String.Empty, MessageType = MessageType.Success });
			
			// assert
			contextMock.Verify(context => context.Request.Cookies, Times.Once());
			contextMock.Verify(context => context.Response.Cookies, Times.Once());
			Assert.IsNotNull(contextMock.Object.Response.Cookies);
			Assert.AreEqual<int>(1, contextMock.Object.Response.Cookies[CookieMessageStorage.StorageKey].Values.Count);
		}

		[TestMethod]
		public void Messenger_CookieStorage_AddMessageToExistingMessages()
		{
			// arrange
			Message success = new Message { MessageType = MessageType.Success, Text = "success" };
			Message warning = new Message { MessageType = MessageType.Warning, Text = "warning" };
			Message error = new Message { MessageType = MessageType.Error, Text = "error" };
			List<Message> messages = new List<Message> { success, warning, error };

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Request.Cookies).Returns(new HttpCookieCollection());
			contextMock.Setup(context => context.Response.Cookies).Returns(new HttpCookieCollection());

			HttpCookie messageCookie = new HttpCookie(CookieMessageStorage.StorageKey);
			for (int i = 0; i < messages.Count; i++)
			{
				string messageToStore = String.Format("{0}|{1}", ((int)messages[i].MessageType).ToString(), HttpUtility.UrlEncode(messages[i].Text));
				messageCookie.Values.Add(i.ToString(), messageToStore);
			}
			contextMock.Object.Request.Cookies.Set(messageCookie);

			// act
			IMessageStorage messageStorage = new CookieMessageStorage(contextMock.Object);
			messageStorage.AddMessage(new Message { Text = String.Empty, MessageType = MessageType.Success });

			// assert
			contextMock.Verify(context => context.Request.Cookies, Times.Exactly(2));
			contextMock.Verify(context => context.Response.Cookies, Times.Once());
			Assert.IsNotNull(contextMock.Object.Response.Cookies);
			Assert.AreEqual<int>(messages.Count + 1, contextMock.Object.Response.Cookies[CookieMessageStorage.StorageKey].Values.Count);
		}

		[TestMethod]
		public void Messenger_CookieStorage_GetMessagesFromRequest()
		{
			// arrange
			Message success = new Message { MessageType = MessageType.Success, Text = "success" };
			Message warning = new Message { MessageType = MessageType.Warning, Text = "warning" };
			Message error = new Message { MessageType = MessageType.Error, Text = "error" };
			List<Message> messages = new List<Message> { success, warning, error };

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Request.Cookies).Returns(new HttpCookieCollection());
			contextMock.Setup(context => context.Response.Cookies).Returns(new HttpCookieCollection());

			HttpCookie messageCookie = new HttpCookie(CookieMessageStorage.StorageKey);
			for (int i = 0; i < messages.Count; i++)
			{
				string messageToStore = String.Format("{0}|{1}", ((int)messages[i].MessageType).ToString(), HttpUtility.UrlEncode(messages[i].Text));
				messageCookie.Values.Add(i.ToString(), messageToStore);
			}
			contextMock.Object.Request.Cookies.Set(messageCookie);

			// act
			IMessageStorage messageStorage = new CookieMessageStorage(contextMock.Object);
			messageStorage.GetMessages();

			// assert
			contextMock.Verify(context => context.Request.Cookies, Times.Exactly(2));
			contextMock.Verify(context => context.Response.Cookies, Times.Never());
			Assert.IsNotNull(contextMock.Object.Response.Cookies);
			Assert.AreEqual<int>(messages.Count, contextMock.Object.Request.Cookies[CookieMessageStorage.StorageKey].Values.Count);
		}

		[TestMethod]
		public void Messenger_CookieStorage_ClearMessages()
		{
			// arrange
			Message success = new Message { MessageType = MessageType.Success, Text = "success" };
			Message warning = new Message { MessageType = MessageType.Warning, Text = "warning" };
			Message error = new Message { MessageType = MessageType.Error, Text = "error" };
			List<Message> messages = new List<Message> { success, warning, error };

			Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
			contextMock.Setup(context => context.Request.Cookies).Returns(new HttpCookieCollection());
			contextMock.Setup(context => context.Response.Cookies).Returns(new HttpCookieCollection());

			HttpCookie messageCookie = new HttpCookie(CookieMessageStorage.StorageKey);
			for (int i = 0; i < messages.Count; i++)
			{
				string messageToStore = String.Format("{0}|{1}", ((int)messages[i].MessageType).ToString(), HttpUtility.UrlEncode(messages[i].Text));
				messageCookie.Values.Add(i.ToString(), messageToStore);
			}
			contextMock.Object.Response.Cookies.Set(messageCookie);

			// act
			IMessageStorage messageStorage = new CookieMessageStorage(contextMock.Object);
			messageStorage.ClearMessages();

			// assert
			contextMock.Verify(context => context.Request.Cookies, Times.Never());
			contextMock.Verify(context => context.Response.Cookies, Times.Exactly(2));
			Assert.IsNotNull(contextMock.Object.Response.Cookies);
			Assert.AreEqual<int>(1, contextMock.Object.Response.Cookies[CookieMessageStorage.StorageKey].Values.Count);
		}
	}
}
