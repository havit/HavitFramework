using System.Collections.Immutable;
using Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.Tests.UnitOfWorks;

[TestClass]
public class UnitOfWorkAddRangeAnalyzerTests
{
	[TestMethod]
	public async Task AddRangeForInsert_WithNestedCollection_ReportsDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			List<List<MyEntity>> nestedList = new List<List<MyEntity>>();
			unitOfWork.AddRangeForInsert(nestedList);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(15, 33)
			.WithArguments("MyEntity", "AddRangeForInsert");

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeForUpdate_WithNestedCollection_ReportsDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			IEnumerable<IEnumerable<MyEntity>> nestedEnumerable = null;
			unitOfWork.AddRangeForUpdate(nestedEnumerable);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(14, 33)
			.WithArguments("MyEntity", "AddRangeForUpdate");

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeForDelete_WithNestedCollection_ReportsDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			var nestedArray = new MyEntity[][] { };
			unitOfWork.AddRangeForDelete(nestedArray);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(14, 33)
			.WithArguments("MyEntity", "AddRangeForDelete");

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeForInsert_WithCorrectCollection_NoDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			List<MyEntity> entities = new List<MyEntity>();
			unitOfWork.AddRangeForInsert(entities);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddRangeForUpdate_WithCorrectIEnumerable_NoDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			IEnumerable<MyEntity> entities = null;
			unitOfWork.AddRangeForUpdate(entities);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddRangeForInsertAsync_WithNestedCollection_ReportsDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public async Task TestMethod(IUnitOfWork unitOfWork)
		{
			List<List<MyEntity>> nestedList = new List<List<MyEntity>>();
			await unitOfWork.AddRangeForInsertAsync(nestedList);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(15, 44)
			.WithArguments("MyEntity", "AddRangeForInsertAsync");

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeForInsert_WithSelectResultingInNestedCollection_ReportsDiagnostic()
	{
		const string source = @"
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			List<List<MyEntity>> source = new List<List<MyEntity>>();
			unitOfWork.AddRangeForInsert(source.Select(x => x));
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(15, 33)
			.WithArguments("MyEntity", "AddRangeForInsert");

		await VerifyAnalyzerAsync(source, expected);
	}

	private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
	{

		var test = new CSharpAnalyzerTest<UnitOfWorkAddRangeAnalyzer, DefaultVerifier>
		{
			TestState =
			{
				Sources = { source },
				ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
			},
		};

		// Add reference to Havit.Data.Patterns
		test.TestState.AdditionalReferences.Add(typeof(Data.Patterns.UnitOfWorks.IUnitOfWork).Assembly);

		test.ExpectedDiagnostics.AddRange(expected);

		await test.RunAsync();
	}
}
