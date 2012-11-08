﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Havit.Business;
using Havit.BusinessLayerTest;
namespace Havit.BusinessTest
{
	/// <summary>
	/// This is a test class for Havit.Business.BusinessObjectCollection&lt;T&gt; and is intended
	/// to contain all Havit.Business.BusinessObjectCollection&lt;T&gt; Unit Tests
	/// </summary>
	[TestClass]
	public class BusinessObjectCollectionTest
	{
		/// <summary>
		/// Testuje výchozí hodnotu AllowDuplicates.
		/// </summary>
		[TestMethod]
		public void AllowDuplicatesTest_Default()
		{
			SubjektCollection subjekty = new SubjektCollection();
			// je AllowDuplicates zapnuto?
			Assert.IsTrue(subjekty.AllowDuplicates);
		}

		/// <summary>
		/// Testuje, zda je při zákazu duplicit ověřeno, zda kolekce již neobsahuje duplicity.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AllowDuplicatesTest_Change()
		{
			SubjektCollection subjekty = new SubjektCollection();
			Subjekt subjekt = Subjekt.CreateObject();

			//přidáme do kolekce dva stejné objekty
			subjekty.Add(subjekt);
			subjekty.Add(subjekt);

			// zapneme test na duplicity
			subjekty.AllowDuplicates = false;
			// je-li vyhozena výjimka, je vše ok (viz atribut metody)			
		}

		/// <summary>
		/// Testuje zákaz duplicit - vkládání pomocí insertu.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AllowDuplicatesTest_DoNotAllow_Insert_Different_New()
		{
			SubjektCollection subjekty = new SubjektCollection();
			subjekty.AllowDuplicates = false;
			Subjekt subjekt = Subjekt.CreateObject();

			// duplicity jsou zakázány
			// přidáme dvakrát stejný subjekt (stejnou instanci)
			subjekty.Add(subjekt);
			subjekty.Add(subjekt);
			// je-li vyhozena výjimka, je vše ok (viz atribut metody)
		}

		/// <summary>
		/// Testuje zákaz duplicit - vkládání pomocí indexeru.
		/// </summary>
		[TestMethod]
		public void AllowDuplicatesTest_Allow_Indexer_Different_New()
		{
			SubjektCollection subjekty = new SubjektCollection();
			subjekty.AllowDuplicates = false;

			Subjekt subjekt1 = Subjekt.CreateObject();
			Subjekt subjekt2 = Subjekt.CreateObject();

			// přidáme dvakrát dva různé objekty, pak přes indexerem první nastavíme sám na sebe
			subjekty.Add(subjekt1);
			subjekty.Add(subjekt2);
			subjekty[0] = subjekt1;			
			// nedošlo-li k výjimce, je vše ok
		}

		/// <summary>
		/// Testuje zákaz duplicit - pomocí indexeru.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AllowDuplicatesTest_DoNotAllow_Insert_Same_Persistent()
		{
			SubjektCollection subjekty = new SubjektCollection();
			subjekty.AllowDuplicates = false;

			Subjekt subjekt1 = Subjekt.GetObject(1);
			Subjekt subjekt2 = Subjekt.GetObject(1);

			// přidáme dva objekty (různé instance, ale stejné business objekty)
			subjekty.Add(subjekt1);
			subjekty.Add(subjekt2);
			// je-li vyhozena výjimka, je vše ok (viz atribut metody)

		}

		/// <summary>
		/// Testuje zákaz duplicit - pomocí indexeru.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]		
		public void AllowDuplicatesTest_DoNotAllow_Indexer_Different()
		{
			SubjektCollection subjekty = new SubjektCollection();
			subjekty.AllowDuplicates = false;

			Subjekt subjekt1 = Subjekt.CreateObject();
			Subjekt subjekt2 = Subjekt.CreateObject();

			// vložím dva různé objekty
			subjekty.Add(subjekt1);
			subjekty.Add(subjekt2);
			// druhý objekt přepíšu prvním
			subjekty[1] = subjekt1;
			// je-li vyhozena výjimka, je vše ok (viz atribut metody)			
		}

		/// <summary>
		/// Testuje povolení duplicit.
		/// </summary>
		[TestMethod]
		public void AllowDuplicatesTest_Allow_Insert_And_Indexer()
		{
			SubjektCollection subjekty;

			Subjekt subjekt = Subjekt.CreateObject();

			subjekty = new SubjektCollection();
			subjekty.AllowDuplicates = true;

			// povoleny duplicity
			// mohu přidávat a nastavovat jeden objekt dokola 

			subjekty.Add(subjekt);
			subjekty.Add(subjekt);

			subjekty[0] = subjekt;
			subjekty[1] = subjekt;
			// nedošlo-li k výjimce, je vše ok
		}

		/// <summary>
		/// Testuje vyvolání události při odebrání prvků z kolekce metodou RemoveRange.
		/// </summary>
		[TestMethod]
		public void RemoveRange_CollectionChanged_Test()
		{
			bool changed = false;

			SubjektCollection collection = new SubjektCollection();
			collection.Add(Subjekt.GetObject(1));
			collection.CollectionChanged += ((sender, args) => changed = true);
			collection.RemoveRange(new SubjektCollection(collection));

			Assert.IsTrue(changed);
		}

	}

}
