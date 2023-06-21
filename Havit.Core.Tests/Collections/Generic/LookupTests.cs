using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Collections.Generic;

[TestClass]
public class LookupTests
{
	[TestMethod]
	public void Lookup_Indexer_ReturnsEnumerableForMissingKey()
	{
		var lookup = new Havit.Collections.Generic.Lookup<object, object>();
		IEnumerable<object> enumerable = lookup["MissingKey"];

		Assert.IsNotNull(enumerable);
		Assert.AreEqual(0, enumerable.Count());
	}

	[TestMethod]
	public void Lookup_Count_ReturnsNumberOfKeys()
	{
		var lookup = new Havit.Collections.Generic.Lookup<int, int>(new int[] { 1, 2, 3 }.ToLookup(i => 0));		

		Assert.AreEqual(1, lookup.Count());
	}
}
