using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.BusinessLayerTest.VnitrostatniDistribuce;
using Havit.BusinessLayerTest;
using Havit.Business;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Unit test generovaných vlastností typu Money.
	/// </summary>
	[TestClass]
	public class MoneyTest
	{
		#region TestAssigmentByParts
		[TestMethod]
		public void MoneyAssignmentByParts()
		{
			using (new IdentityMapScope())
			{
				decimal castka = 555M;
				Currency mena = Currency.GetAll().First();

				TarifHmotnostItem tarifHmotnostItem = TarifHmotnostItem.CreateObject();
				tarifHmotnostItem.Cena.Amount = castka;
				tarifHmotnostItem.Cena.Currency = mena;

				Assert.AreEqual(tarifHmotnostItem.Cena.Amount, castka);
				Assert.AreEqual(tarifHmotnostItem.Cena.Currency, mena);
			}
		}
		#endregion

		#region MoneyInstanceAssignment
		[TestMethod]
		public void MoneyInstanceAssignment()
		{
			using (new IdentityMapScope())
			{
				decimal castka = 555M;
				Currency mena = Currency.GetAll().First();
				
				TarifHmotnostItem tarifHmotnostItem = TarifHmotnostItem.CreateObject();

				Money money = new Money(castka, mena);
				tarifHmotnostItem.Cena = money;

				Assert.AreEqual(tarifHmotnostItem.CenaAmount, castka);
				Assert.AreEqual(tarifHmotnostItem.CenaCurrency, mena);
				Assert.AreSame(money, tarifHmotnostItem.Cena);
			}
		}
		#endregion

		#region MoneyInstanceChange
		[TestMethod]
		public void MoneyInstanceChange()
		{
			using (new IdentityMapScope())
			{
				decimal castka1 = 1M;
				decimal castka2 = 2M;
				Currency mena1 = Currency.GetAll().First();
				Currency mena2 = Currency.GetAll().Skip(1).First();

				TarifHmotnostItem tarifHmotnostItem = TarifHmotnostItem.CreateObject();

				Money money = new Money(castka1, mena1);
				tarifHmotnostItem.Cena = money;

				Assert.AreEqual(tarifHmotnostItem.CenaAmount, castka1);
				Assert.AreEqual(tarifHmotnostItem.CenaCurrency, mena1);

				tarifHmotnostItem.CenaAmount = castka2;
				tarifHmotnostItem.CenaCurrency = mena2;

				Assert.AreEqual(tarifHmotnostItem.Cena.Amount, castka2);
				Assert.AreEqual(tarifHmotnostItem.Cena.Currency, mena2);

				money = new Money(castka1, mena1);
				tarifHmotnostItem.Cena = money;

				Assert.AreEqual(tarifHmotnostItem.CenaAmount, castka1);
				Assert.AreEqual(tarifHmotnostItem.CenaCurrency, mena1);

				Assert.AreSame(money, tarifHmotnostItem.Cena);
			}
		}
		#endregion

	}
}
