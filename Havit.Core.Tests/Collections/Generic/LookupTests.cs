namespace Havit.Core.Tests.Collections.Generic;

[TestClass]
public class LookupTests
{
	[TestMethod]
	public void Lookup_Indexer_ReturnsEnumerableForMissingKey()
	{
		var lookup = new Havit.Collections.Generic.Lookup<object, object>();
		IEnumerable<object> enumerable = lookup["MissingKey"];

		Assert.IsNotNull(enumerable);
		Assert.IsEmpty(enumerable);
	}

	[TestMethod]
	public void Lookup_Count_ReturnsNumberOfKeys()
	{
		var lookup = new Havit.Collections.Generic.Lookup<int, int>(new int[] { 1, 2, 3 }.ToLookup(i => 0));

		Assert.HasCount(1, lookup);
	}
}
