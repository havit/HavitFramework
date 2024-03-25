using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Ares.FinancniSprava;
using Havit.Diagnostics.Contracts;
using Moq;

namespace Havit.Ares.Tests.FinancniSprava;

[TestClass]
public class AresDphServiceTests
{
	[TestMethod]
	[TestCategory("AresDph")]
	public void AresDphService_GetAresAndPlatceDph_PlatceDph()
	{
		// Arrange
		Mock<IAresService> aresServiceMock = new Mock<IAresService>(MockBehavior.Strict);
		aresServiceMock.Setup(m => m.GetEkonomickeSubjektyDleIco(It.IsAny<string>())).Returns(new EkonomickySubjektItem
		{
			EkonomickySubjektAres = new EkonomickySubjekt
			{
				Dic = "DIC123"
			},
			EkonomickySubjektExtension = new EkonomickySubjektExtension
			{
				IsPlatceDph = true,
			}
		});

		Mock<IPlatceDphService> platceDphServiceMock = new Mock<IPlatceDphService>(MockBehavior.Strict);
		platceDphServiceMock.Setup(m => m.GetPlatceDph(It.IsAny<string>())).Returns(new PlatceDphResult());

		var aresDphService = new AresDphService(aresServiceMock.Object, platceDphServiceMock.Object);

		// Act
		_ = aresDphService.GetAresAndPlatceDph("ICO123");

		// Act
		platceDphServiceMock.Verify(m => m.GetPlatceDph("DIC123"), Times.Once);
	}


	[TestMethod]
	[TestCategory("AresDph")]
	public void AresDphService_GetAresAndPlatceDph_NeplatceDph()
	{
		// Arrange
		Mock<IAresService> aresServiceMock = new Mock<IAresService>(MockBehavior.Strict);
		aresServiceMock.Setup(m => m.GetEkonomickeSubjektyDleIco(It.IsAny<string>())).Returns(new EkonomickySubjektItem
		{
			EkonomickySubjektAres = new EkonomickySubjekt
			{
				Dic = "DIC123"
			},
			EkonomickySubjektExtension = new EkonomickySubjektExtension
			{
				IsPlatceDph = false,
			}
		});

		Mock<IPlatceDphService> platceDphServiceMock = new Mock<IPlatceDphService>(MockBehavior.Strict);
		platceDphServiceMock.Setup(m => m.GetPlatceDph(It.IsAny<string>())).Returns(new PlatceDphResult());

		var aresDphService = new AresDphService(aresServiceMock.Object, platceDphServiceMock.Object);

		// Act
		_ = aresDphService.GetAresAndPlatceDph("ICO123");

		// Act
		platceDphServiceMock.Verify(m => m.GetPlatceDph("DIC123"), Times.Never);
	}
}
