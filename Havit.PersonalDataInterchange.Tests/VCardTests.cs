using System.Text;

namespace Havit.PersonalDataInterchange.Tests;

[TestClass]
public class VCardTests
{
	[TestMethod]
	public void WriteToStream_BasicTest_DefaultEncoding()
	{
		// Arrange
		var vCard = new VCard
		{
			N = new VCard.ContactName
			{
				LastName = "Doe",
				FirstName = "John",
				MiddleName = "M",
				Title = "Mr.",
				Suffix = "Jr."
			},
			FN = "John Doe",
			Title = "Manager",
			Org = "Havit",
			Email = "john.doe@example.com",
			EmailPref = "john.doe@example.com",
			TelCell = "1234567890",
			Fax = "9876543210",
			Url = "https://www.example.com",
			TelWork = "5555555555",
			TelHome = "9999999999",
			AdrWork = "123 Main St, City, State, Country",
			IsAdrWorkAsQuotedPrintable = false,
			AdrHome = "456 Second St, City, State, Country",
			IsAdrHomeAsQuotedPrintable = true,
			Note = "This is a test note"
		};

		using (var stream = new MemoryStream())
		using (var writer = new StreamWriter(stream))
		{
			// Act
			vCard.WriteToStream(writer);
			writer.Flush();
			stream.Position = 0;
			var reader = new StreamReader(stream);
			var vCardContent = reader.ReadToEnd();

			// Assert
			Assert.AreEqual("BEGIN:VCARD\r\nVERSION:2.1\r\nN;CHARSET=UTF-8:Doe;John;M;Mr.;Jr.\r\nFN;CHARSET=UTF-8:John Doe\r\nTITLE;CHARSET=UTF-8:Manager\r\nORG;CHARSET=UTF-8:Havit\r\nEMAIL;PREF;INTERNET:john.doe@example.com\r\nEMAIL;INTERNET:john.doe@example.com\r\nTEL;CELL;VOICE:1234567890\r\nTEL;WORK;FAX:9876543210\r\nURL;WORK:https://www.example.com\r\nTEL;WORK;VOICE:5555555555\r\nTEL;HOME;VOICE:9999999999\r\nADR;WORK;PREF;CHARSET=UTF-8:;;123 Main St, City, State, Country\r\nADR;HOME;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:;;456 Second St, City, State, Country\r\nNOTE;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:This is a test note\r\nEND:VCARD\r\n", vCardContent);
		}
	}

	[TestMethod]
	public void WriteToStream_Windows1250()
	{
		// Arrange
		var vCard = new VCard
		{
			N = new VCard.ContactName
			{
				LastName = "Doe",
				FirstName = "John",
				MiddleName = "M",
				Title = "Mr.",
				Suffix = "Jr."
			},
			FN = "John Doe",
			Title = "Manager",
			Org = "Havit",
			Email = "john.doe@example.com",
			EmailPref = "john.doe@example.com",
			TelCell = "1234567890",
			Fax = "9876543210",
			Url = "https://www.example.com",
			TelWork = "5555555555",
			TelHome = "9999999999",
			AdrWork = "123 Main St, City, State, Country",
			IsAdrWorkAsQuotedPrintable = false,
			AdrHome = "456 Second St, City, State, Country",
			IsAdrHomeAsQuotedPrintable = true,
			Note = "This is a test note"
		};

		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		using (var stream = new MemoryStream())
		using (var writer = new StreamWriter(stream, Encoding.GetEncoding(1250)))
		{
			// Act
			vCard.WriteToStream(writer);
			writer.Flush();
			stream.Position = 0;
			var reader = new StreamReader(stream);
			var vCardContent = reader.ReadToEnd();

			// Assert
			Assert.AreEqual("BEGIN:VCARD\r\nVERSION:2.1\r\nN;CHARSET=WINDOWS-1250:Doe;John;M;Mr.;Jr.\r\nFN;CHARSET=WINDOWS-1250:John Doe\r\nTITLE;CHARSET=WINDOWS-1250:Manager\r\nORG;CHARSET=WINDOWS-1250:Havit\r\nEMAIL;PREF;INTERNET:john.doe@example.com\r\nEMAIL;INTERNET:john.doe@example.com\r\nTEL;CELL;VOICE:1234567890\r\nTEL;WORK;FAX:9876543210\r\nURL;WORK:https://www.example.com\r\nTEL;WORK;VOICE:5555555555\r\nTEL;HOME;VOICE:9999999999\r\nADR;WORK;PREF;CHARSET=WINDOWS-1250:;;123 Main St, City, State, Country\r\nADR;HOME;CHARSET=WINDOWS-1250;ENCODING=QUOTED-PRINTABLE:;;456 Second St, City, State, Country\r\nNOTE;CHARSET=WINDOWS-1250;ENCODING=QUOTED-PRINTABLE:This is a test note\r\nEND:VCARD\r\n", vCardContent);
		}
	}
}
