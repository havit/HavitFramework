using Havit.BusinessLayerTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Business.Tests
{
	[TestClass]
	public class CollectionPropertyHolderTests
	{
		[TestMethod]
		public void CollectionPropertyHolder_Initialize_CanBeRunMultipleTimes()
		{
			using (new IdentityMapScope())
			{
				// Arrange
				Role role = Role.CreateDisconnectedObject();
				CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);

				// Act
				collectionPropertyHolder.Initialize("1|2|3|"); // i za poslední položkou musí být oddělovač!
				var tmp = collectionPropertyHolder.Value; // force initialization
				collectionPropertyHolder.Initialize("3|4|5|"); // i za poslední položkou musí být oddělovač!

				// Assert
				Assert.AreEqual(3, collectionPropertyHolder.Value.Count);
				Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 3));
				Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 4));
				Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 5));
			}
		}
	}
}
