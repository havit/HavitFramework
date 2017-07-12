using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Business;
using Havit.BusinessLayerTest;
using Havit.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.BusinessTest
{
	[TestClass]
	public class BusinessObject_PersistenceTest
	{
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BusinessObject_Load_ThrowsExceptionForNotExistingObject()
		{
			using (new IdentityMapScope())
			{
				Role.GetObject(-999).Load();
			}
		}

		[TestMethod]
		public void BusinessObject_TryLoad_ReturnsTrueForGhostOfExistingObjectAndForLoadedObject()
		{
			using (new IdentityMapScope())
			{
				Role role = Role.GetObject(0);
				Assert.IsTrue(role.TryLoad());
				Assert.IsTrue(role.TryLoad());
			}
		}

		[TestMethod]
		public void BusinessObject_TryLoad_ReturnsFalseForNotExistingObject()
		{
			using (new IdentityMapScope())
			{
				Role role = Role.GetObject(999);
				Assert.IsFalse(role.TryLoad());
			}
		}

		[TestMethod]
		public void BusinessObject_TryLoad_WithTransaction_ReloadsObject()
		{
			using (new IdentityMapScope())
			{
				// Arrange
				Uzivatel uzivatel = Uzivatel.CreateObject();
				uzivatel.Username = Guid.NewGuid().ToString();
				uzivatel.Save();

				DbParameter parameter = DbConnector.Default.ProviderFactory.CreateParameter();
				parameter.ParameterName = "@UzivatelID";
				parameter.DbType = System.Data.DbType.Int32;
				parameter.Direction = System.Data.ParameterDirection.Input;
				parameter.Value = uzivatel.ID;

				DbCommand cmd = DbConnector.Default.ProviderFactory.CreateCommand();
				cmd.CommandText = "UPDATE Uzivatel SET Email = 'hfw@havit.local' WHERE UzivatelID = @UzivatelID";
				cmd.Parameters.Add(parameter);

				DbConnector.Default.ExecuteNonQuery(cmd);

				// Předpoklad:
				Assert.AreEqual(String.Empty, uzivatel.Email);

				// Act
				DbConnector.Default.ExecuteTransaction(transaction => uzivatel.TryLoad(transaction));

				// Assert
				Assert.AreEqual("hfw@havit.local", uzivatel.Email);
				
				// Cleanup
				uzivatel.Delete();
			}
		}

		[TestMethod]
		public void BusinessObject_TryLoad_WithoutTransaction_DoesNotReloadObject()
		{
			using (new IdentityMapScope())
			{
				// Arrange
				Uzivatel uzivatel = Uzivatel.CreateObject();
				uzivatel.Username = Guid.NewGuid().ToString();
				uzivatel.Save();

				DbParameter parameter = DbConnector.Default.ProviderFactory.CreateParameter();
				parameter.ParameterName = "@UzivatelID";
				parameter.DbType = System.Data.DbType.Int32;
				parameter.Direction = System.Data.ParameterDirection.Input;
				parameter.Value = uzivatel.ID;

				DbCommand cmd = DbConnector.Default.ProviderFactory.CreateCommand();
				cmd.CommandText = "UPDATE Uzivatel SET Email = 'hfw@havit.local' WHERE UzivatelID = @UzivatelID";
				cmd.Parameters.Add(parameter);

				DbConnector.Default.ExecuteNonQuery(cmd);

				// Předpoklad:
				Assert.AreEqual(String.Empty, uzivatel.Email);

				// Act
				uzivatel.TryLoad();

				// Assert
				Assert.AreEqual(String.Empty, uzivatel.Email);

				// Cleanup
				uzivatel.Delete();
			}
		}

		[TestMethod]
		public void BusinessObject_Delete_DeletesObjectAlreadySavedInTheSameTransaction()
		{
			using (new IdentityMapScope())
			{
				int originalCount = Subjekt.GetAll().Count;
				Subjekt subjekt = Subjekt.CreateObject();
				subjekt.Nazev = "test";
				subjekt.Save();
				DbConnector.Default.ExecuteTransaction(transaction =>
				{
					subjekt.Nazev = "test2";
					subjekt.Save(transaction);
					subjekt.Delete(transaction);
				});
				int newCount = Subjekt.GetAll().Count;
				Assert.AreEqual(originalCount, newCount);
			}
		}

		[TestMethod]
		public void BusinessObject_Delete_DoesNotCallCheckConstraint()
		{
			using (new IdentityMapScope())
			{
				DbConnector.Default.ExecuteTransaction(transaction =>
				{
					Subjekt subjekt = Subjekt.CreateObject();
					subjekt.Save(transaction);
					string s = "";
					while (s.Length <= Subjekt.Properties.Nazev.MaximumLength)
					{
						s = s + "0";
					}

					subjekt.Nazev = s;
					subjekt.Delete(transaction);
				});
			}
		}

		/// <summary>
		/// Nový objekt ukládá cyklický graf, kde sám by měl být minimal-insertován od jiného objektu, který je Update.
		/// </summary>
		[TestMethod]
		public void BusinessObject_Save_SupportsCyclicUpdateWithInsert()
		{
			using (new IdentityMapScope())
			{
				Subjekt s = Subjekt.CreateObject();
				s.Save();

				Komunikace k1 = Komunikace.CreateObject();
				k1.Subjekt = s;
				s.Komunikace.Add(k1);
				k1.Save();

				ObjednavkaSepsani o1 = ObjednavkaSepsani.CreateObject();
				k1.ObjednavkaSepsani = o1;
				k1.Save();

				Komunikace k2 = Komunikace.CreateObject();
				k2.Subjekt = s;
				s.Komunikace.Add(k2);

				o1.StornoKomunikace = k2;
				k2.Save();

				Assert.IsFalse(k2.IsDirty);
				Assert.IsFalse(k2.IsNew);
				Assert.IsFalse(o1.IsDirty);
			}
		}

		/// <summary>
		/// Pokud máme objekt, na který jsou navázané další objekty, otestujeme možnost smazání.
		/// </summary>
		[TestMethod]
		public void BusinessObject_Delete_SupportsDeletingObjectWithSavingChanges()
		{
			using (new IdentityMapScope())
			{
				Uzivatel uzivatel = Uzivatel.CreateObject();
				uzivatel.Username = DateTime.Now.Ticks.ToString("0");
				uzivatel.Role.Add(Role.GetAll().First());
				uzivatel.Save();

				uzivatel.Role.Clear();
				uzivatel.Delete();
			}
		}
	}
}
