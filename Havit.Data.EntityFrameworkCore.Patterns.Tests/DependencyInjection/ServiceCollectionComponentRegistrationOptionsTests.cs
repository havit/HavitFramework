using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection
{
	[TestClass]
	public class ServiceCollectionComponentRegistrationOptionsTests
	{
		/// <summary>
		/// Pokud jsme používali jako type pro lifestyle ServiceLifetime, nezafungovaly defaulty pro RepositoriesLifestyle, protoře jde o strukturu (enum), tedy nemá hodnotu null.
		/// Tím se použila výchozí hodnota pro lifestyles Singleton a nepoužila se výchozí GeneralLifestyle.
		/// 
		/// </summary>
		[TestMethod]
		public void ServiceCollectionComponentRegistrationOptions_RepositoriesLifestyle_DefaultValueIsGeneralLifestyle()
		{
			// Arrange
			// Act
			ServiceCollectionComponentRegistrationOptions options = new ServiceCollectionComponentRegistrationOptions();

			// Assert
			Assert.AreNotEqual(options.GeneralLifestyle, Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton); // nesmí to být singleton, jinak následujícím řádkem nic nepoznáme
			Assert.AreEqual(options.GeneralLifestyle, options.RepositoriesLifestyle);
		}
	}
}
