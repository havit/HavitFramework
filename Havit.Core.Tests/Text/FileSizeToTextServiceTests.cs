using System.Globalization;
using Havit.Text;

namespace Havit.Tests.Text;

[TestClass]
public class FileSizeToTextServiceTests
{
	[TestMethod]
	public void FileSizeToTextService_GetFileSizeToText()
	{
		// arrange
		FileSizeToTextService service = new FileSizeToTextService();

		CultureInfo originalCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // kvůli desetinnému oddělovači

			// act + assert

			// do 1023 B (včetně) se zobrazuje velikost v bajtech
			Assert.AreEqual("0 B", service.GetFileSizeToText(0));
			Assert.AreEqual("1023 B", service.GetFileSizeToText(1023));

			// hodnoty se nezaokrouhlují, ale ořezávají (floor)
			Assert.AreEqual("1 kB", service.GetFileSizeToText(1024));
			Assert.AreEqual("1.99 kB", service.GetFileSizeToText(2047));
			Assert.AreEqual("10.6 kB", service.GetFileSizeToText((long)(10.65 * 1024)));
			Assert.AreEqual("100 kB", service.GetFileSizeToText(100 * 1024));

			// koncové nuly se nezobrazují (nikdy 1.10, 2.00)
			Assert.AreEqual("1.1 kB", service.GetFileSizeToText(1127)); // 1127/1024 = 1.10058...
			Assert.AreEqual("2 kB", service.GetFileSizeToText(2048));
			Assert.AreEqual("10 kB", service.GetFileSizeToText(10 * 1024));

			// od 1000 jednotek se použije vyšší jednotka
			Assert.AreEqual("0.97 MB", service.GetFileSizeToText(1000 * 1024));
			Assert.AreEqual("0.97 GB", service.GetFileSizeToText(1000L * 1024 * 1024));
			Assert.AreEqual("0.97 TB", service.GetFileSizeToText(1000L * 1024 * 1024 * 1024));

			// terabajty
			Assert.AreEqual("1 TB", service.GetFileSizeToText(1024L * 1024 * 1024 * 1024));
			Assert.AreEqual("1.5 TB", service.GetFileSizeToText((long)(1.5 * 1024 * 1024 * 1024 * 1024)));

			// nejvyšší jednotkou je terabyte (s oddělovačem tisíců dle kultury)
			Assert.AreEqual("2,048 TB", service.GetFileSizeToText(2048L * 1024 * 1024 * 1024 * 1024));
		}
		finally
		{
			CultureInfo.CurrentCulture = originalCulture;
		}
	}
}
