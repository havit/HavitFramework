using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbExtensionsIncludeTests
	{
		[TestMethod]
		public void DbExtensionsInclude_CheckSupportechPatterns()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			// Act

			// classic select
			dbContext.Set<Child>().Include(c => c.Parent).ThenInclude(p => p.ChildrenIncludingDeleted).ThenInclude(c => c.Parent).FirstOrDefault();
			dbContext.Set<Child>().Include(c => c.Parent.ChildrenIncludingDeleted).ThenInclude(c => c.Parent).FirstOrDefault();
			dbContext.Set<HiearchyItem>().Include(c1 => c1.Children).ThenInclude(c2 => c2.Children).ThenInclude(c3 => c3.Children).FirstOrDefault();
			
			// Předchozí způsob načítání již není podporován
			//dbContext.Set<Child>().Include(c => c.Parent.Children.Select(item => item.Parent)).FirstOrDefault();
			//dbContext.Set<HiearchyItem>().Include(c1 => c1.Children.Select(c2 => c2.Children.Select(c3 => c3.Children))).FirstOrDefault();

			// Assert
			// no exception was thrown
		}
	}
}
