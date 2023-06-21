using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Services.Tests;

[TestClass]
public class ServiceFactoryExtensionsTests
{
	[TestMethod]
	public void ServiceFactoryExtensions_ExecuteAction_CreatesService()
	{
		// arrange
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

		// act
		serviceFactory.Object.ExecuteAction(service =>
		{
			// NOOP
		});

		// assert
		serviceFactory.Verify(f => f.CreateService(), Times.Once);
	}

	[TestMethod]
	public void ServiceFactoryExtensions_ExecuteAction_ExecutesAction()
	{
		// arrange
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

		// act
		object result = null;
		serviceFactory.Object.ExecuteAction(service =>
		{
			result = service;
		});

		// assert
		Assert.AreSame(fakeService, result);
	}

	[TestMethod]
	public void ServiceFactoryExtensions_ExecuteAction_ReleasesService()
	{
		// arrange
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

		// act
		serviceFactory.Object.ExecuteAction(service =>
		{
			// NOOP
		});

		// assert
		serviceFactory.Verify(f => f.ReleaseService(fakeService), Times.Once);
	}

	[TestMethod]
	[ExpectedException(typeof(Exception), "FAKE EXCEPTION")]
	public void ServiceFactoryExtensions_ExecuteAction_ActionThrowsException_ReleasesService()
	{
		// arrange
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);

		// act
		try
		{
			serviceFactory.Object.ExecuteAction(service =>
			{
				throw new Exception("FAKE EXCEPTION");
			});
		}
		finally
		{
			// assert
			serviceFactory.Verify(f => f.ReleaseService(fakeService), Times.Once);
		}
	}

	[TestMethod]
	public void ServiceFactoryExtensions_CreateDisposableService_CreatesService()
	{
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);


#pragma warning disable S108 // Nested blocks of code should not be left empty
#pragma warning disable SA1501 // Statement must not be on a single line
		using (serviceFactory.Object.CreateDisposableService())
		{ }
#pragma warning restore SA1501 // Statement must not be on a single line
#pragma warning restore S108 // Nested blocks of code should not be left empty

		serviceFactory.Verify(f => f.CreateService(), Times.Once);
	}

	[TestMethod]
	public void ServiceFactoryExtensions_CreateDisposableService_ReleasesService()
	{
		var fakeService = new object();
		var serviceFactory = new Mock<IServiceFactory<object>>();
		serviceFactory.Setup(f => f.CreateService()).Returns(fakeService);


#pragma warning disable S108 // Nested blocks of code should not be left empty
#pragma warning disable SA1501 // Statement must not be on a single line
		using (serviceFactory.Object.CreateDisposableService())
		{ }
#pragma warning restore SA1501 // Statement must not be on a single line
#pragma warning restore S108 // Nested blocks of code should not be left empty

		serviceFactory.Verify(f => f.ReleaseService(fakeService), Times.Once);
	}
}