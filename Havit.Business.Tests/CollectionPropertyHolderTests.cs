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
			Assert.Contains(item => item.ID == 3, collectionPropertyHolder.Value);
			Assert.Contains(item => item.ID == 4, collectionPropertyHolder.Value);
			Assert.Contains(item => item.ID == 5, collectionPropertyHolder.Value);
		}
	}

	[TestMethod]
	public void CollectionPropertyHolder_Value_ParsesShortItemIDs()
	{
		using (new IdentityMapScope())
		{
			// Arrange
			Role role = Role.CreateDisconnectedObject();
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);

			// Act
			collectionPropertyHolder.Initialize("1|22|333|");
			int[] actual = collectionPropertyHolder.Value.Select(item => item.ID).ToArray();

			// Assert
			Assert.AreSequenceEqual(new int[] { 1, 22, 333 }, actual);
		}
	}

	[TestMethod]
	public void CollectionPropertyHolder_Value_ParsesLongItemIDs()
	{
		using (new IdentityMapScope())
		{
			// Arrange
			Role role = Role.CreateDisconnectedObject();
			int[] expected = Enumerable.Range(1, 1000).Select(i => i * 997).ToArray();
			string itemIDsWithDelimiter = String.Concat(expected.Select(id => id + "|"));
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);

			// Act
			collectionPropertyHolder.Initialize(itemIDsWithDelimiter);
			int[] actual = collectionPropertyHolder.Value.Select(item => item.ID).ToArray();

			// Assert
			Assert.AreSequenceEqual(expected, actual);
		}
	}

	[TestMethod]
	public void CollectionPropertyHolder_Value_ParsesNegativeItemIDs()
	{
		using (new IdentityMapScope())
		{
			// Arrange
			Role role = Role.CreateDisconnectedObject();
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);

			// Act
			collectionPropertyHolder.Initialize("-1|0|-22|333|-123456789|");
			int[] actual = collectionPropertyHolder.Value.Select(item => item.ID).ToArray();

			// Assert
			Assert.AreSequenceEqual(new int[] { -1, 0, -22, 333, -123456789 }, actual);
		}
	}
}
