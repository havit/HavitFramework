using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Business;
using Havit.BusinessLayerTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.Tests;

[TestClass]
public class BusinessObject_EqualityTests
{
	[TestMethod]
	public void BusinessObject_Equals_EqualsOnSameInstances()
	{
		// Arrange
		Role role1;
		Subjekt subjekt1;

		using (new IdentityMapScope())
		{
			role1 = Role.GetObject(-1);
		}
		subjekt1 = Subjekt.CreateObject();

		// Act + Assert
		Assert.IsTrue(role1.Equals(role1));			
#pragma warning disable 1718 // cílem je otestovat porovnání se sebou samým
		Assert.IsTrue(role1 == role1);
#pragma warning restore 1718

		Assert.IsTrue(subjekt1.Equals(subjekt1));
#pragma warning disable 1718 // cílem je otestovat porovnání se sebou samým
		Assert.IsTrue(subjekt1 == subjekt1);
#pragma warning restore 1718
	}

	[TestMethod]
	public void BusinessObject_Equals_EqualsGetObjectWithSameID()
	{
		// Arrange
		Role role1;
		Role role2;
		Role role3;

		using (new IdentityMapScope())
		{
			role1 = Role.GetObject(-1);
			role2 = Role.GetObject(-1);
		}

		using (new IdentityMapScope())
		{
			role3 = Role.GetObject(-1);
		}

		// Act + Assert

		// porovnání objektů v jedné identity mapě
		Assert.IsTrue(role1.Equals(role2));
		Assert.IsTrue(role1 == role2);

		Assert.IsTrue(role1.Equals(role1));
#pragma warning disable 1718 // cílem je otestovat porovnání se sebou samým
		Assert.IsTrue(role1 == role1);
#pragma warning restore 1718
		// porovnání objektů v různých identity mapách
		Assert.IsTrue(role1.Equals(role3));
		Assert.IsTrue(role1 == role3);

	}

	[TestMethod]
	public void BusinessObject_Equals_DoesNotEqualObjectsOfDifferentTypeWithSameID()
	{
		// Arrange
		Role role1;
		Subjekt subjekt1;

		using (new IdentityMapScope())
		{
			role1 = Role.GetObject(-1);
			subjekt1 = Subjekt.GetObject(-1);
		}

		// Act + Assert
		// porovnání objektů v jedné identity mapě
		Assert.IsFalse(role1.Equals(subjekt1));
		Assert.IsFalse(role1 == subjekt1);

	}

	[TestMethod]
	public void BusinessObject_Equals_DoesNoEqualGetObjectWithDifferentIDs()
	{
		// Arrange
		Role role1;
		Role role2;
		Role role3;

		using (new IdentityMapScope())
		{
			role1 = Role.GetObject(-1);
			role2 = Role.GetObject(1);
		}

		using (new IdentityMapScope())
		{
			role3 = Role.GetObject(1);
		}

		// Act + Assert

		// porovnání objektů v jedné identity mapě
		Assert.IsFalse(role1.Equals(role2));
		Assert.IsFalse(role1 == role2);

		// porovnání objektů v různých identity mapách
		Assert.IsFalse(role1.Equals(role3));
		Assert.IsFalse(role1 == role3);
	}

	[TestMethod]
	public void BusinessObject_Equals_DoesNoEqualNewObjects()
	{
		// Arrange
		Subjekt subjekt1 = Subjekt.CreateObject();
		Subjekt subjekt2 = Subjekt.CreateObject();

		// Act
		Assert.IsFalse(subjekt1.Equals(subjekt2));
		Assert.IsFalse(subjekt1 == subjekt2);
	}

}