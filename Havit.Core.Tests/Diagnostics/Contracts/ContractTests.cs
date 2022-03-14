using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Diagnostics.Contracts
{
	[TestClass]
	public class ContractTests
	{
		[TestMethod]
		public void Contract_Requires_NoMessage()
		{
			// Act
			ContractException contractException = null;
			try
			{
				int i = 0;
				Contract.Requires(i == (5 + 5)); // vždy false, použito jako expression pro net6.0
			}
			catch (ContractException e)
			{
				contractException = e;
			}

			// Assert
			Assert.IsNotNull(contractException);
			Assert.AreEqual("Contract failed: i == (5 + 5)", contractException.Message);
		}

		[TestMethod]
		public void Contract_Requires_WithMessage()
		{
			// Act
			ContractException contractException = null;
			try
			{
				Contract.Requires(false, "Custom message.");
			}
			catch (ContractException e)
			{
				contractException = e;
			}

			// Assert
			Assert.IsNotNull(contractException);
			Assert.AreEqual("Contract failed: Custom message.", contractException.Message);
		}

		[TestMethod]
		public void Contract_GenericRequires_ArgumentException_WithMessage()
		{
			// Act
			ArgumentException argumentException = null;
			try
			{
				Contract.Requires<ArgumentException>(false, "Custom message.");
			}
			catch (ArgumentException e)
			{
				argumentException = e;
			}

			// Assert
			Assert.IsNotNull(argumentException);
			Assert.AreEqual("Custom message.", argumentException.Message);
		}

		[TestMethod]
		public void Contract_GenericRequires_ArgumentNullException_WithMessage()
		{
			// Act
			ArgumentNullException argumentException = null;
			try
			{
				Contract.Requires<ArgumentNullException>(false, "Custom message.");
			}
			catch (ArgumentNullException e)
			{
				argumentException = e;
			}

			// Assert
			Assert.IsNotNull(argumentException);
			Assert.AreEqual("Custom message.", argumentException.Message);
		}
	}
}