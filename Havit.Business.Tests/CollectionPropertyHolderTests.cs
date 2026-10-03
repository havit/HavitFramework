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
			collectionPropertyHolder.Initialize("1|22|333|"); // kratší než 25 znaků
			int[] actual = collectionPropertyHolder.Value.Select(item => item.ID).ToArray();

			// Assert
			CollectionAssert.AreEqual(new int[] { 1, 22, 333 }, actual);
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
			string itemIDsWithDelimiter = String.Concat(expected.Select(id => id + "|")); // delší než 25 znaků
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);

			// Act
			collectionPropertyHolder.Initialize(itemIDsWithDelimiter);
			int[] actual = collectionPropertyHolder.Value.Select(item => item.ID).ToArray();

			// Assert
			CollectionAssert.AreEqual(expected, actual);
		}
	}

	[TestMethod]
	public void CollectionPropertyHolder_Value_ParsesLongItemIDsAfterLongerInput()
	{
		// buffer pro parsování je půjčován z ArrayPool, ověřujeme, že zbytek dat z předchozího (delšího) vstupu nemá vliv na výsledek

		using (new IdentityMapScope())
		{
			// Arrange
			Role role = Role.CreateDisconnectedObject();
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder1 = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);
			CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization> collectionPropertyHolder2 = new CollectionPropertyHolder<RoleLocalizationCollection, RoleLocalization>(role, RoleLocalization.GetObject);
			collectionPropertyHolder1.Initialize("100001|100002|100003|100004|100005|100006|100007|100008|");
			_ = collectionPropertyHolder1.Value; // force initialization

			// Act
			collectionPropertyHolder2.Initialize("1|2|3|4|5|6|7|8|9|10|11|12|");
			int[] actual = collectionPropertyHolder2.Value.Select(item => item.ID).ToArray();

			// Assert
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, actual);
		}
	}
}
