using Havit.Data.EntityFrameworkCore.Patterns.Analyzers.FilteringCollections;
using Havit.Model.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.Tests.FilteringCollections;

[TestClass]
public class FilteringCollectionInExpressionTreeAnalyzerTests
{
	/// <summary>
	/// Model, na kterém jsou postaveny všechny testy: <c>Master.Children</c> je FilteringCollection nad namapovanou
	/// kolekcí <c>Master.ChildrenIncludingDeleted</c>, <c>Master.Others</c> je FilteringCollection bez namapovaného protějšku.
	/// Součástí je i <c>Include</c> extension metoda se stejnou signaturou, jakou má EF Core (aby testy nemusely
	/// referencovat EF Core) a metoda přijímající Expression mimo IQueryable (tvar, jaký má např. FluentValidation RuleFor).
	/// </summary>
	private const string ModelDeclarations = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataLoaders;
using Havit.Model.Collections.Generic;

namespace TestNamespace
{
	public class Child
	{
		public int Id { get; set; }
		public DateTime? Deleted { get; set; }
		public Master Master { get; set; }
	}

	public class Master
	{
		public int Id { get; set; }
		public List<Child> ChildrenIncludingDeleted { get; } = new List<Child>();
		public FilteringCollection<Child> Children { get; }
		public FilteringCollection<Child> Others { get; }

		public Master()
		{
			Children = new FilteringCollection<Child>(ChildrenIncludingDeleted, child => child.Deleted == null);
			Others = new FilteringCollection<Child>(ChildrenIncludingDeleted, child => child.Deleted != null);
		}
	}

	public static class QueryableExtensions
	{
		public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) => source;
	}

	public static class Validation
	{
		public static void RuleFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
		{
		}
	}
}
";

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_WhereWithAnyOnFilteringCollection_ReportsDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Where(master => {|#0:master.Children|}.Any());
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Children", "'ChildrenIncludingDeleted' with an explicit filter"));
	}

	/// <summary>
	/// Nejzrádnější případ: ve finální projekci EF Core nehlásí chybu, jen kolekci vůbec nenačte a vrátí prázdno.
	/// </summary>
	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_CountInProjection_ReportsDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Select(master => new { master.Id, Count = {|#0:master.Children|}.Count });
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Children", "'ChildrenIncludingDeleted' with an explicit filter"));
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_NestedSubqueryInProjection_ReportsDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Select(master => new { Ids = {|#0:master.Children|}.Select(child => child.Id).ToList() });
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Children", "'ChildrenIncludingDeleted' with an explicit filter"));
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_IncludeOfFilteringCollection_ReportsDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Include(master => {|#0:master.Children|});
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Children", "'ChildrenIncludingDeleted' with an explicit filter"));
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_ExpressionVariable_ReportsDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public Expression<Func<Master, bool>> GetFilter()
		{
			return master => {|#0:master.Children|}.Any();
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Children", "'ChildrenIncludingDeleted' with an explicit filter"));
	}

	/// <summary>
	/// Kolekce bez namapovaného protějšku <c>XIncludingDeleted</c> - hláška nemá co navrhnout jménem.
	/// </summary>
	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_WithoutIncludingDeletedCounterpart_ReportsDiagnosticWithFallbackSuggestion()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Where(master => {|#0:master.Others|}.Any());
		}
	}
}";

		await VerifyAnalyzerAsync(source, ExpectedDiagnostic("Others", "the underlying mapped collection with an explicit filter"));
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_MappedCollection_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IQueryable<Master> masters)
		{
			masters.Where(master => master.ChildrenIncludingDeleted.Any(child => child.Deleted == null));
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_DataLoaderLoad_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IDataLoader dataLoader, Master master)
		{
			dataLoader.Load(master, item => item.Children);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_DataLoaderLoadAllAsync_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public async Task TestMethod(IDataLoader dataLoader, IEnumerable<Master> masters)
		{
			await dataLoader.LoadAllAsync(masters, item => item.Children);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_DataLoaderThenLoad_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IDataLoader dataLoader, IEnumerable<Child> children)
		{
			dataLoader.LoadAll(children, child => child.Master).ThenLoad(master => master.Children);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	/// <summary>
	/// Cizí API přijímající Expression (např. FluentValidation RuleFor) není dotaz do databáze - hlásit se nemá.
	/// </summary>
	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_ExpressionOutsideQuery_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod()
		{
			Validation.RuleFor<Master, IEnumerable<Child>>(master => master.Children);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_InMemoryLinq_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(List<Master> masters)
		{
			masters.Where(master => master.Children.Any()).ToList();
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task FilteringCollectionInExpressionTreeAnalyzer_DirectAccess_DoesNotReportDiagnostic()
	{
		const string source = ModelDeclarations + @"
namespace TestNamespace
{
	public class TestClass
	{
		public int TestMethod(Master master)
		{
			return master.Children.Count;
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	private static DiagnosticResult ExpectedDiagnostic(string propertyName, string suggestion)
	{
		return new DiagnosticResult(Analyzers.Diagnostics.FilteringCollectionInExpressionTree)
			.WithLocation(0)
			.WithArguments(propertyName, suggestion);
	}

	private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
	{
		var test = new CSharpAnalyzerTest<FilteringCollectionInExpressionTreeAnalyzer, DefaultVerifier>
		{
			TestState =
			{
				Sources = { source },
				ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
			},
		};

		test.TestState.AdditionalReferences.Add(typeof(FilteringCollection<>).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(Data.Patterns.DataLoaders.IDataLoader).Assembly);

		test.ExpectedDiagnostics.AddRange(expected);

		await test.RunAsync();
	}
}
