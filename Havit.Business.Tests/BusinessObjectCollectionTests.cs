using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Havit.Business;
using Havit.BusinessLayerTest;
namespace Havit.Business.Tests
{
	[TestClass]
	public class BusinessObjectCollectionTests
	{
		/// <summary>
		/// Testuje výchozí hodnotu AllowDuplicates.
		/// </summary>
		[TestMethod]
		public void BusinessObjectCollection_AllowDuplicates_IsTrueByDefault()
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
		public void BusinessObjectCollection_AllowDuplicates_ThrowsExceptionWhenSetToFalseAndDuplicityExists()
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
		public void BusinessObjectCollection_Add_ThrowsExceptionWhenAddingDuplicityAndAllowDuplicatesDisabled()
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
		/// Testuje zákaz duplicit - pomocí indexeru.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void BusinessObjectCollection_SetIndexer_ThrowsExceptionWhenAddingDuplicityAndAllowDuplicatesDisabled()
		{
			using (new IdentityMapScope())
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
		}

		/// <summary>
		/// Testuje zákaz duplicit - pomocí indexeru.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void BusinessObjectBase_SetIndexer_ThrowsExceptionWhenAddingDuplicityAndAllowDuplicatesDisabled()
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
		public void BusinessObjectBase_AddAndSetIndexer_AllowsAddDuplicityWhenAllowDuplicatesIsTrue()
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
		public void BusinessObjectCollection_RemoveRange_CallsCollectionChanged()
		{
			using (new IdentityMapScope())
			{
				bool changed = false;

				SubjektCollection collection = new SubjektCollection();
				collection.Add(Subjekt.GetObject(1));
				collection.CollectionChanged += ((sender, args) => changed = true);
				collection.RemoveRange(new SubjektCollection(collection));

				Assert.IsTrue(changed);
			}
		}

		/// <summary>
		/// Testuje vyvolání výjimky při odebrání prvků ze zamčené kolekce metodou Clear.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BusinessObjectCollection_Clear_ThrowsExceptionOnFreezedCollection()
		{
			SubjektCollection collection = new SubjektCollection();
			collection.Freeze();
			collection.Clear();
		}

		/// <summary>
		/// Testuje zda clean na prázdné kolekci nenastavuje objekt dirty.
		/// </summary>
		[TestMethod]
		public void BusinessObjectCollection_Clear_DoesNotSetObjectDirtyOnEmtpyCollection()
		{
			using (new IdentityMapScope())
			{
				// Arrange
				Subjekt subjekt = Subjekt.GetAll().First(item => item.Komunikace.Count == 0);

				// Precondition
				Assert.IsFalse(subjekt.IsDirty);

				// Act
				subjekt.Komunikace.Clear();

				// Assert
				Assert.IsFalse(subjekt.IsDirty);
			}
		}
	}
}
