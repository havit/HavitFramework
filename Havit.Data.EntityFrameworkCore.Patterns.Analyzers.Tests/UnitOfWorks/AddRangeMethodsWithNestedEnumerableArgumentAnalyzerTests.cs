using System.Collections.Immutable;
using Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.Tests.UnitOfWorks;

[TestClass]
public class AddRangeMethodsWithNestedEnumerableArgumentAnalyzerTests
{
	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForInsert_ReportsDiagnosticForNestedEnumerable()
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
			List<List<MyEntity>> nestedList = new List<List<MyEntity>>();
			unitOfWork.AddRangeForInsert(nestedList);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddRangeNestedCollection)
			.WithLocation(14, 33)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddRangeForInsertMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForInsertAsync_ReportsDiagnosticForNestedEnumerable()
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
			.WithArguments("MyEntity", UnitOfWorkConstants.AddRangeForInsertAsyncMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForUpdate_ReportsDiagnosticForNestedEnumerable()
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
			.WithArguments("MyEntity", UnitOfWorkConstants.AddRangeForUpdateMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForDelete_ReportsDiagnosticForNestedEnumerable()
	{
		const string source = @"
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
			.WithLocation(13, 33)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddRangeForDeleteMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForInsert_NoDiagnosticForEnumerable()
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
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForInsertAsync_NoDiagnosticForEnumerable()
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
			List<MyEntity> entities = new List<MyEntity>();
			await unitOfWork.AddRangeForInsertAsync(entities);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForUpdate_NoDiagnosticForEnumerable()
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
	public async Task AddRangeMethodsWithNestedEnumerableArgumentAnalyzer_AddRangeForDelete_NoDiagnosticForEnumerable()
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
			unitOfWork.AddRangeForDelete(entities);
		}
	}
}";
		await VerifyAnalyzerAsync(source);
	}

	private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
	{

		var test = new CSharpAnalyzerTest<AddRangeMethodsWithNestedEnumerableArgumentAnalyzer, DefaultVerifier>
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
