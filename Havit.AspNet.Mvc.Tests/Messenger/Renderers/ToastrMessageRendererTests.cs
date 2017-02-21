using Havit.AspNet.Mvc.Messenger;
using Havit.AspNet.Mvc.Messenger.Renderers;
using Havit.AspNet.Mvc.Messenger.Storages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Tests.Messenger.Renderers
{
	[TestClass]
	public class ToastrMessageRendererTests
	{
		[TestMethod]
		public void ToastrMessageRenderer_RenderMessage_RenderedStringIsNotNull()
		{
			// arrange
			Mock<IMessageStorage> messageStorage = new Mock<IMessageStorage>();
			messageStorage.Setup(x => x.GetMessages()).Returns(new List<Message> { new Message { Text = "test", MessageType = MessageType.Info } });
			
			// act
			IMessageRenderer render = new ToastrMessageRenderer(messageStorage.Object);
			MvcHtmlString renderedString = render.Render();
			
			// assert
			Assert.IsFalse(MvcHtmlString.IsNullOrEmpty(renderedString));
		}

		[TestMethod]
		public void ToastrMessageRenderer_RenderMessage_MessageIsHtmlEncoded()
		{
			// arrange
			Mock<IMessageStorage> messageStorage = new Mock<IMessageStorage>();
			messageStorage.Setup(x => x.GetMessages()).Returns(new List<Message> { new Message { Text = "This should be \"HTML\" encoded.", MessageType = MessageType.Info } });

			// act
			IMessageRenderer render = new ToastrMessageRenderer(messageStorage.Object);
			MvcHtmlString renderedString = render.Render();

			// assert
			Assert.AreEqual("<script type=\"text/javascript\">toastr.info(\"This should be &quot;HTML&quot; encoded.\");\r\n</script>", renderedString.ToString());
		}
	}
}
