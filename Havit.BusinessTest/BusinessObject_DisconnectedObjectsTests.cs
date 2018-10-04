using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Business;
using Havit.Business.Query;
using Havit.Business.TestExtensions;
using Havit.BusinessLayerTest;
using Havit.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.BusinessTest
{
	[TestClass]
	public class BusinessObject_DisconnectedObjectsTests
	{
		/// <summary>
		/// Testuje, že není možné vytvořit instanci disconnected objektů těch objektů, které již jsou v identity mapě.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BusinessObject_CreateDisconnectedObject_CannotBeCreatedWhenExistsInIdentityMap()
		{
			using (new IdentityMapScope())
			{
				Subjekt.GetObject(1);
				Subjekt.CreateDisconnectedObject(1);
			}
		}

		/// <summary>
		/// Testuje, že GetObject vrací z identity mapy i disconnected object.
		/// </summary>
		[TestMethod]
		public void BusinessObject_GetObjects_ReturnsDisconnectedObjectWhenCreatedDisconnected()
		{
			using (new IdentityMapScope())
			{
				var subjektCreated = Subjekt.CreateDisconnectedObject(1);
				var subjektFromIdentityMap = Subjekt.GetObject(1);
				Assert.AreSame(subjektCreated, subjektFromIdentityMap);
			}
		}

		/// <summary>
		/// Testuje a ověřuje nastavení vlastností disconnected business objektu po jeho založení.
		/// </summary>
		[TestMethod]
		public void BusinessObject_CreateDisconnectedObject_WithID_IsDisconnectedAndDirtyAndLoadedAndNotNew()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateDisconnectedObject(1);
				Assert.IsTrue(subjekt.IsDisconnected, "Objekt by měl být disconnected.");
				Assert.IsFalse(subjekt.IsNew, "Objekt by neměl být označen, jako nový.");
				Assert.IsTrue(subjekt.IsDirty, "Objekt bude označen, jako dirty - jsou mu nastaveny výchozí hodnoty.");
				Assert.IsTrue(subjekt.IsLoaded, "Objekt by měl být považován za načtený.");
				Assert.IsFalse(subjekt.IsDeleted, "Objekt by neměl být považován za smazaný.");
			}
		}

		/// <summary>
		/// Testuje a ověřuje nastavení vlastností business objektu po jeho založení.
		/// </summary>
		[TestMethod]
		public void BusinessObject_CreateObject_IsDirtyAndLoadedAndNewAndNotDisconnected()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateObject();
				Assert.IsFalse(subjekt.IsDisconnected);
				Assert.IsTrue(subjekt.IsNew);
				Assert.IsTrue(subjekt.IsDirty);
				Assert.IsTrue(subjekt.IsLoaded);
				Assert.IsFalse(subjekt.IsDeleted);
			}
		}

		/// <summary>
		/// Testuje, že disconnected objekt se nenačítá z databáze (volání bez transakce).
		/// </summary>
		[TestMethod]
		public void BusinessObject_TryLoad_WithoutTransaction_DoesNotLoadFromDatabaseForDisconnectedObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateDisconnectedObject(-999);
				Assert.IsTrue(subjekt.TryLoad());
			}
		}

		/// <summary>
		/// Testuje, že disconnected objekt se nenačítá z databáze (volání s transakcí).
		/// </summary>
		[TestMethod]
		public void BusinessObject_TryLoad_WithTransaction_DoesNotLoadFromDatabaseForDisconnectedObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateDisconnectedObject(-999);
				DbConnector.Default.ExecuteTransaction(transaction => { Assert.IsTrue(subjekt.TryLoad(transaction)); });
			}
		}

		/// <summary>
		/// Testuje, že zavoláním metody SetDisconnected se objekt přepne do stavu disconnected objektu.
		/// </summary>
		[TestMethod]
		public void BusinessObject_SetDisconnected_DisconnectsNewObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateObject();
				subjekt.SetDisconnected();
				Assert.IsTrue(subjekt.IsDisconnected);
			}
		}

		/// <summary>
		/// Testuje, že zavoláním metody SetDisconnected se objekt přepne do stavu disconnected objektu.
		/// </summary>
		[TestMethod]
		public void BusinessObject_SetDisconnected_DisconnectsGhostObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.GetObject(-999);
				subjekt.SetDisconnected();
				Assert.IsTrue(subjekt.IsDisconnected);
			}
		}

		/// <summary>
		/// Testuje, že zavoláním metody SetDisconnected se objekt přepne do stavu disconnected objektu.
		/// </summary>
		[TestMethod]
		public void BusinessObject_SetDisconnected_DisconnectsGhostObjectDisconnectsLoadedObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateObject();
				subjekt.Save();
				subjekt.SetDisconnected();
				Assert.IsTrue(subjekt.IsDisconnected);
			}
		}

		/// <summary>
		/// Testuje, že zavoláním metody SetProperty se nastaví hodnota vlastnosti.
		/// </summary>
		[TestMethod]
		public void BusinessObject_SetPropertyExtensionMethod_SetsPropertyToReadonlyObject()
		{
			using (new IdentityMapScope())
			{
				Role role = Role.CreateDisconnectedObject();
				role.SetProperty(item => item.Symbol, "AAA");

				Assert.AreEqual("AAA", role.Symbol);
			}
		}

		// BugRepro 27250
		[TestMethod]
		public void BusinessObject_SetPropertyExtensionMethod_NullableProperty_SetsPropertyValue()
		{
			using (new IdentityMapScope())
			{
				// arrange
				Subjekt subjekt = Subjekt.CreateDisconnectedObject();
				DateTime? value = new DateTime?(new DateTime(2016, 7, 15));

				// act
				subjekt.SetProperty(item => item.Deleted, value);

				// assert
				Assert.AreEqual(value, subjekt.Deleted);
			}
		}

		/// <summary>
		/// Testuje, že zavolání metody SetProperty není možné na Ghost objektu.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BusinesObject_SetPropertyExtensionMethod_ThrowsExceptionForGhostObject()
		{
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.GetObject(-999);
				subjekt.SetProperty(item => item.Nazev, "AAA");
			}
		}

		/// <summary>
		/// Testuje, že disconnected objekt není uložen.
		/// </summary>
		[TestMethod]
		public void BusinesObject_Save_DoNotSaveNewDisconnectedObjects()
		{
			// protože se smysluplně neumím nabourat do SaveFullInsert, který potřebuheme potlačit (a který je generovaný), 
		    // implementujeme test jako black-box test
			using (new IdentityMapScope())
			{
				Subjekt subjekt = Subjekt.CreateDisconnectedObject();

				using (var connection = DbConnector.Default.GetConnection(true))
				{
					DbTransaction dbTransaction = connection.BeginTransaction();

					QueryParams qp = new QueryParams();
					qp.Properties.Add(Subjekt.Properties.ID); // omezíme zátěž db - načteme jen ghosts

					int count1 = Subjekt.GetList(qp, dbTransaction).Count;
					subjekt.Save(dbTransaction); // Act
					int count2 = Subjekt.GetList(qp, dbTransaction).Count;

					dbTransaction.Rollback();

					Assert.AreEqual(count1, count2, "Došlo k uložení objektu do databáze.");
				}
			}
		}

		/// <summary>
		/// Testuje, že disconnected objekt není uložen.
		/// </summary>
		[TestMethod]
		public void BusinesObject_Save_DoNotSaveNewDisconnectedMembers()
		{
			// protože se smysluplně neumím nabourat do SaveFullInsert, který potřebuheme potlačit (a který je generovaný), 
			// implementujeme test jako black-box test
			using (new IdentityMapScope())
			{
				CenikItem cenikItem = CenikItem.CreateDisconnectedObject();
				cenikItem.CenaCurrency = Currency.CreateDisconnectedObject();
				cenikItem.CenaAmount = 5;

				using (var connection = DbConnector.Default.GetConnection(true))
				{
					DbTransaction dbTransaction = connection.BeginTransaction();

					QueryParams qp = new QueryParams();
					qp.Properties.Add(Currency.Properties.ID); // omezíme zátěž db - načteme jen ghosts

					int count1 = Currency.GetList(qp, dbTransaction).Count;
					cenikItem.Save(dbTransaction); // Act
					int count2 = Currency.GetList(qp, dbTransaction).Count;

					dbTransaction.Rollback();

					Assert.AreEqual(count1, count2, "Došlo k uložení objektu do databáze.");
				}
			}
		}
	}
}
