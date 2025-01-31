﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;
namespace Havit.Business.Tests;

[TestClass]
public class ReferenceConditionTests
{
	[TestMethod]
	public void ReferenceCondition_CreateEquals_ReturnsEmptyListForNonExistingObject()
	{
		IOperand operand = Subjekt.Properties.Uzivatel;
		int? id = -10145603;  // neexistující uživatel

		QueryParams qp = new QueryParams();
		qp.Conditions.Add(ReferenceCondition.CreateEquals(operand, id));
		SubjektCollection subjekty = Subjekt.GetList(qp);

		Assert.AreEqual(subjekty.Count, 0);
	}

	/// <summary>
	/// Test CreateIn na prázdné pole. Vrací StaticCondition, která se vždy vyhodnotí jak neplatná podmínka.
	/// </summary>
	[TestMethod]
	public void ReferenceCondition_CreateIn_ReturnsStaticConditionForEmptyArray()
	{
		IOperand operand = ValueOperand.Create(0);

		Condition condition = ReferenceCondition.CreateIn(operand, new int[] { });
		Assert.IsTrue(condition is StaticCondition); // interní třída
	}

	/// <summary>
	/// Test CreateIn na pole o jedné hodnotě. Vrací BinaryCondition testující rovnost hodnoty.
	/// </summary>
	[TestMethod]
	public void ReferenceCondition_CreateIn_ReturnsBinaryConditionForOneIDArray()
	{
		IOperand operand = ValueOperand.Create(0);

		Condition condition = ReferenceCondition.CreateIn(operand, new int[] { 1 });
		Assert.IsTrue(condition is BinaryCondition);
	}

	/// <summary>
	/// Test CreateIn na pole o více hodnotách, přičemž interval je spojiný. Vrací TernaryCondition testující hodnotu v rozmezí (between).
	/// </summary>
	[TestMethod]
	public void ReferenceCondition_CreateIn_ReturnsTernaryConditionForContinuousArrayOfIDs()
	{
		IOperand operand = ValueOperand.Create(0);

		Condition condition = ReferenceCondition.CreateIn(operand, new int[] { 1, 3, 2, 0 });
		Assert.IsTrue(condition is TernaryCondition);
	}

	/// <summary>
	/// Test CreateIn na pole o více hodnotách, přičemž interval není spojiný. Vrací ReferenceInCondition.
	/// </summary>
	[TestMethod]
	public void ReferenceCondition_CreateIn_ReturnsInIntegerConditionForRandomArrayOfIDs()
	{
		IOperand operand = ValueOperand.Create(0);

		Condition condition = ReferenceCondition.CreateIn(operand, new int[] { 1, 3, 5, 7 });
		Assert.IsTrue(condition is InIntegersCondition);
	}

	/// <summary>
	/// Test CreateIn na pole o jedné hodnotě, byť duplikované. Vrací BinaryCondition.
	/// </summary>
	[TestMethod]
	public void ReferenceCondition_CreateIn_IgnoresDuplicatesInArray()
	{
		IOperand operand = ValueOperand.Create(0);

		Condition condition = ReferenceCondition.CreateIn(operand, new int[] { 1, 1, 1 });
		Assert.IsTrue(condition is BinaryCondition);
	}
}
