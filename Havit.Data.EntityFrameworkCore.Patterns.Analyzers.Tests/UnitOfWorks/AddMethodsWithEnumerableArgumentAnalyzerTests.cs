using Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.Tests.UnitOfWorks;

[TestClass]
public class AddMethodsWithEnumerableArgumentAnalyzerTests
{
	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_ReportsDiagnosticForIEnumerable()
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
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			unitOfWork.AddForInsert(entities);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(14, 28)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForInsertMethodName, UnitOfWorkConstants.AddRangeForInsertMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsertAsync_ReportsDiagnosticForIEnumerable()
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
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			await unitOfWork.AddForInsertAsync(entities);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(15, 39)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForInsertAsyncMethodName, UnitOfWorkConstants.AddRangeForInsertAsyncMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForUpdate_ReportsDiagnosticForIEnumerable()
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
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			unitOfWork.AddForUpdate(entities);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(14, 28)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForUpdateMethodName, UnitOfWorkConstants.AddRangeForUpdateMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForDelete_ReportsDiagnosticForIEnumerable()
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
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			unitOfWork.AddForDelete(entities);
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(15, 28)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForDeleteMethodName, UnitOfWorkConstants.AddRangeForDeleteMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_NoDiagnosticForEntity()
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
			MyEntity entity = new MyEntity();
			unitOfWork.AddForInsert(entity);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsertAsync_NoDiagnosticForEntity()
	{
		const string source = @"
using System.Threading.Tasks;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class TestClass
	{
		public async Task TestMethod(IUnitOfWork unitOfWork)
		{
			MyEntity entity = new MyEntity();
			await unitOfWork.AddForInsertAsync(entity);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForUpdate_NoDiagnosticForEntity()
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
			MyEntity entity = new MyEntity();
			unitOfWork.AddForUpdate(entity);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForDelete_NoDiagnosticForEntity()
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
			MyEntity entity = new MyEntity();
			unitOfWork.AddForDelete(entity);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	private static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
	{
		var test = new CSharpAnalyzerTest<AddMethodsWithEnumerableArgumentAnalyzer, DefaultVerifier>
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