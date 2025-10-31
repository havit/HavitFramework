using Havit.BusinessLayerTest;

namespace Havit.Business.Tests;

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
			Assert.HasCount(3, collectionPropertyHolder.Value);
			Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 3));
			Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 4));
			Assert.IsTrue(collectionPropertyHolder.Value.Any(item => item.ID == 5));
		}
	}
}
