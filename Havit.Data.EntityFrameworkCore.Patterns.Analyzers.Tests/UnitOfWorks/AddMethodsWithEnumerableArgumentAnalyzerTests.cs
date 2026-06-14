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

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_ReportsDiagnosticForArray()
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
			MyEntity[] entities = new MyEntity[0];
			unitOfWork.AddForInsert({|#0:entities|});
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(0)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForInsertMethodName, UnitOfWorkConstants.AddRangeForInsertMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_ReportsDiagnosticForNamedArgument()
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
			unitOfWork.AddForInsert(entity: {|#0:entities|});
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(0)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForInsertMethodName, UnitOfWorkConstants.AddRangeForInsertMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_ReportsDiagnosticForConcreteUnitOfWork()
	{
		const string source = @"
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class MyEntity { }

	public class MyUnitOfWork : IUnitOfWork
	{
		public void Commit() { }
		public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
		public void AddForInsert<TEntity>(TEntity entity) where TEntity : class { }
		public ValueTask AddForInsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class => default;
		public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class { }
		public ValueTask AddRangeForInsertAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class => default;
		public void AddForUpdate<TEntity>(TEntity entity) where TEntity : class { }
		public void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class { }
		public void AddForDelete<TEntity>(TEntity entity) where TEntity : class { }
		public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class { }
		public void RegisterAfterCommitAction(Action action) { }
		public void RegisterAfterCommitAction(Func<CancellationToken, Task> asyncAction) { }
		public void Clear() { }
	}

	public class TestClass
	{
		public void TestMethod(MyUnitOfWork unitOfWork)
		{
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			unitOfWork.AddForInsert({|#0:entities|});
		}
	}
}";

		var expected = new DiagnosticResult(Analyzers.Diagnostics.UnitOfWorkAddIEnumerableArgument)
			.WithLocation(0)
			.WithArguments("MyEntity", UnitOfWorkConstants.AddForInsertMethodName, UnitOfWorkConstants.AddRangeForInsertMethodName);

		await VerifyAnalyzerAsync(source, expected);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_NoDiagnosticForString()
	{
		// string implements IEnumerable<char> but must not be treated as a collection of entities.
		const string source = @"
using Havit.Data.Patterns.UnitOfWorks;

namespace TestNamespace
{
	public class TestClass
	{
		public void TestMethod(IUnitOfWork unitOfWork)
		{
			string value = ""text"";
			unitOfWork.AddForInsert(value);
		}
	}
}";

		await VerifyAnalyzerAsync(source);
	}

	[TestMethod]
	public async Task AddMethodsWithEnumerableArgumentAnalyzer_AddForInsert_NoDiagnosticForNonUnitOfWorkType()
	{
		// A same-named method on a type that is not IUnitOfWork must not be flagged.
		const string source = @"
using System.Collections.Generic;

namespace TestNamespace
{
	public class MyEntity { }

	public class NotAUnitOfWork
	{
		public void AddForInsert<TEntity>(TEntity entity) where TEntity : class { }
	}

	public class TestClass
	{
		public void TestMethod(NotAUnitOfWork service)
		{
			IEnumerable<MyEntity> entities = new List<MyEntity>();
			service.AddForInsert(entities);
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