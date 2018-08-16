using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Services.Tests
{
	[TestClass]
	public class ServiceFactoryDisposableWrapperTests
	{
		[TestMethod]
		public void ServiceFactoryDisposableWrapperTests_CreatesService()
		{
			var fakeService = new object();
			var serviceFactory = new Mock<IServiceFactory<object>>();
			serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

#pragma warning disable S108 // Nested blocks of code should not be left empty
#pragma warning disable SA1501 // Statement must not be on a single line
			using (new ServiceFactoryDisposableWrapper<object>(serviceFactory.Object))
			{ }
#pragma warning restore SA1501 // Statement must not be on a single line
#pragma warning restore S108 // Nested blocks of code should not be left empty

			serviceFactory.Verify(f => f.CreateService(), Times.Once);
		}

		[TestMethod]
		public void ServiceFactoryDisposableWrapperTests_ReleasesService()
		{
			var fakeService = new object();
			var serviceFactory = new Mock<IServiceFactory<object>>();
			serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

#pragma warning disable S108 // Nested blocks of code should not be left empty
#pragma warning disable SA1501 // Statement must not be on a single line
			using (new ServiceFactoryDisposableWrapper<object>(serviceFactory.Object))
			{ }
#pragma warning restore SA1501 // Statement must not be on a single line
#pragma warning restore S108 // Nested blocks of code should not be left empty

			serviceFactory.Verify(f => f.ReleaseService(fakeService), Times.Once);
		}

		[TestMethod]
		public void ServiceFactoryDisposableWrapperTests_HasProperService()
		{
			var fakeService = new object();
			var serviceFactory = new Mock<IServiceFactory<object>>();
			serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

			using (var service = new ServiceFactoryDisposableWrapper<object>(serviceFactory.Object))
			{
				Assert.AreSame(fakeService, service.Service);
			}
		}

		[TestMethod]
		public void ServiceFactoryDisposableWrapperTests_HasProperServiceImplicitCast()
		{
			var fakeService = "jiri";
			var serviceFactory = new Mock<IServiceFactory<string>>();
			serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

			using (var service = new ServiceFactoryDisposableWrapper<string>(serviceFactory.Object))
			{
				void Assert(string expected, string actual)
				{
					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(expected, actual);
				}
				Assert(fakeService, service);
			}
		}
	}
}