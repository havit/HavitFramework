using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.BusinessLayerTest;

namespace Havit.Business.Tests;

[TestClass]
public class ActiveRecordBusinessObjectBaseTests
{
	/// <summary>
	/// Defect 329: CheckConstraints je obejito, pokud je objekt uložen přes MinimalInsert.
	/// </summary>
	[TestMethod]
	[ExpectedException(typeof(ConstraintViolationException))]
	public void ActiveRecordBusinessObjectBase_MinimalInsert_UsesCheckConstraint()
	{
		// Arrange
		Subjekt subjekt = Subjekt.CreateObject();
		Uzivatel uzivatel = Uzivatel.CreateObject();
		subjekt.Uzivatel = uzivatel;

		// nastavíme řetězec delší, než je povoleno
		string username = "";
		while (username.Length <= Uzivatel.Properties.Username.MaximumLength)
		{
			username = username + "0";
		}
		uzivatel.Username = username;

		// Act
		subjekt.Save();

		// Assert
		// uživatel musí být otestován pomocí CheckConstraint, očekáváme výjimku ConstraintViolationException (viz atribut této metody)
	}
}
