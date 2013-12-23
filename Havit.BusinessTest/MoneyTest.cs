using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

				CenikItem cenikItem = CenikItem.CreateObject();
				cenikItem.Cena.Amount = castka;
				cenikItem.Cena.Currency = mena;

				Assert.AreEqual(cenikItem.Cena.Amount, castka);
				Assert.AreEqual(cenikItem.Cena.Currency, mena);
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
				
				CenikItem cenikItem = CenikItem.CreateObject();

				Money money = new Money(castka, mena);
				cenikItem.Cena = money;

				Assert.AreEqual(cenikItem.CenaAmount, castka);
				Assert.AreEqual(cenikItem.CenaCurrency, mena);
				Assert.AreSame(money, cenikItem.Cena);
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

				CenikItem cenikItem = CenikItem.CreateObject();

				Money money = new Money(castka1, mena1);
				cenikItem.Cena = money;

				Assert.AreEqual(cenikItem.CenaAmount, castka1);
				Assert.AreEqual(cenikItem.CenaCurrency, mena1);

				cenikItem.CenaAmount = castka2;
				cenikItem.CenaCurrency = mena2;

				Assert.AreEqual(cenikItem.Cena.Amount, castka2);
				Assert.AreEqual(cenikItem.Cena.Currency, mena2);

				money = new Money(castka1, mena1);
				cenikItem.Cena = money;

				Assert.AreEqual(cenikItem.CenaAmount, castka1);
				Assert.AreEqual(cenikItem.CenaCurrency, mena1);

				Assert.AreSame(money, cenikItem.Cena);
			}
		}
		#endregion

	}
}
