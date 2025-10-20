namespace Havit.Data.EntityFrameworkCore.Tests;

[TestClass]
public class QueryTagBuilderTests
{
	[TestMethod]
	public void QueryTagBuilder_CreateTag()
	{
		// Arrange
		// Act + Assert
		Assert.AreEqual("QueryTagBuilderTests.TestMember", QueryTagBuilder.CreateTag(typeof(QueryTagBuilderTests), "TestMember")); // explicit memberName
		Assert.AreEqual("QueryTagBuilderTests", QueryTagBuilder.CreateTag(typeof(QueryTagBuilderTests), null)); // explicit memberName, but null
		Assert.AreEqual("QueryTagBuilderTests.QueryTagBuilder_CreateTag", QueryTagBuilder.CreateTag(typeof(QueryTagBuilderTests))); // default value for memberName by CallerMemberNameAttribute
	}
}
