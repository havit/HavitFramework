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
	public class BusinessObject_MoneyPropertyTests
	{
		[TestMethod]
		public void BusinessObject_MoneyProperty_AmountAndCurrencyPropertiesAreSetAfterSetMoneyProperty()
		{
			using (new IdentityMapScope())
			{
				decimal castka = 555M;
				Currency mena = Currency.CreateObject();
				
				CenikItem cenikItem = CenikItem.CreateObject();

				Money money = new Money(castka, mena);
				cenikItem.Cena = money;

				Assert.AreEqual(cenikItem.CenaAmount, castka);
				Assert.AreEqual(cenikItem.CenaCurrency, mena);

				Assert.AreSame(money, cenikItem.Cena); // podmínka není funkčně nutná, je ale výhodná z paměťových a výkonostních důvodů
			}
		}

		[TestMethod]
		public void BusinessObject_MoneyProperty_MoneyPropertyIsSetAfterSettingAmountAndCurrencyProperties()
		{
			using (new IdentityMapScope())
			{
				decimal castka1 = 1M;
				decimal castka2 = 2M;
				Currency mena1 = Currency.CreateObject();
				Currency mena2 = Currency.CreateObject();

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

				Assert.AreSame(money, cenikItem.Cena); // podmínka není funkčně nutná, je ale výhodná z paměťových a výkonostních důvodů
			}
		}

	}
}
