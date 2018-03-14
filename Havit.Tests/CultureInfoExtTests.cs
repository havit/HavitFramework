using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Tests
{
	[TestClass]
	public class CultureInfoExtTests
	{
		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_ExecutesAction()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("cs-CZ");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.FakeMethod()).Verifiable();

			// act
			cultureInfoStub.ExecuteMethod(mockClass.Object.FakeMethod);

			// assert
			mockClass.Verify(m => m.FakeMethod(), Times.Once);
		}

		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_ExecutesFunc()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("cs-CZ");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.FakeMethodReturnsObject()).Returns(new object()).Verifiable();

			// act
			var returnedObject = cultureInfoStub.ExecuteMethod(mockClass.Object.FakeMethodReturnsObject);

			// assert
			mockClass.Verify(m => m.FakeMethodReturnsObject(), Times.Once);
			Assert.IsNotNull(returnedObject);
		}

		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_ReturnsMethodReturnParameter()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("cs-CZ");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.FakeMethodReturnsObject()).Returns(new object()).Verifiable();

			// act
			var returnedObject = cultureInfoStub.ExecuteMethod(mockClass.Object.FakeMethodReturnsObject);

			// assert
			mockClass.Verify(m => m.FakeMethodReturnsObject(), Times.Once);
			Assert.IsNotNull(returnedObject);
		}

		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_GetCurrentCultureName_ReturnsCzechCulture()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("cs-CZ");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.GetCurrentCultureName()).CallBase().Verifiable();

			// act
			string currentCultureName = cultureInfoStub.ExecuteMethod(mockClass.Object.GetCurrentCultureName);

			// assert
			Assert.AreEqual("cs-CZ", currentCultureName);
		}

		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_GetCurrentCultureName_ReturnsEnglishCulture()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("en-US");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.GetCurrentCultureName()).CallBase().Verifiable();

			// act
			string currentCultureName = cultureInfoStub.ExecuteMethod(mockClass.Object.GetCurrentCultureName);

			// assert
			Assert.AreEqual("en-US", currentCultureName);
		}

		[TestMethod]
		public void CultureInfoExt_ExecuteMethod_GetCurrentCultureName_ReturnsSlovakCulture()
		{
			// arrange
			CultureInfo cultureInfoStub = new CultureInfo("sk-SK");
			var mockClass = new Mock<FakeClassForCultureTest>();
			mockClass.Setup(m => m.GetCurrentCultureName()).CallBase().Verifiable();

			// act
			string currentCultureName = cultureInfoStub.ExecuteMethod(mockClass.Object.GetCurrentCultureName);

			// assert
			Assert.AreEqual("sk-SK", currentCultureName);
		}

		public class FakeClassForCultureTest
		{
			public virtual void FakeMethod()
			{
				
			}

			public virtual object FakeMethodReturnsObject()
			{
				return new object();
			}

			public virtual string GetCurrentCultureName()
			{
				return CultureInfo.CurrentCulture.Name;
			}
		}
	}
}
