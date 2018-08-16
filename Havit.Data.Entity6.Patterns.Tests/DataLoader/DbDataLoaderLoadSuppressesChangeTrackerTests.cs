using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;
using Havit.Data.Entity.Tests.Helpers;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader
{
	///// <summary>
	///// Při opakovaném volání dbDataLoader.Load se opakovaně volal change tracker.
	///// Ten způsobil výkonový problém.
	///// V testu tedy zkusíme na velkém množství objektů v paměti ověřit, že je algoritmus stále dostatečně rychlý. Viz konstruktor.
	///// Situace se nejlépe ověřuje (největší rozdíl v rychlosti je) pro persistentní trackované objekty. Pro nové objekty není rozdíl tak patrný, 
	///// tj. pokud bychom odstranili save changes a druhý DbContext, test by také fungoval, ale rozdíly S a BEZ changetrackeru nejsou tak veliké (ale jsou znatelné).
	///// </summary>
	//[TestClass]
	//public class DbDataLoaderLoadSuppressesChangeTrackerTests
	//{
	//	[ClassCleanup]
	//	public static void CleanUp()
	//	{
	//		DeleteDatabaseHelper.DeleteDatabase<DataLoaderTestDbContext>();
	//	}

	//	/// <summary>
	//	/// Proč v samostatné třídě s inicializací v konstruktoru? Pro korektnější měření času, viz 
	//	/// http://stackoverflow.com/questions/7815534/how-can-i-precisely-time-a-test-run-in-visual-studio-2010
	//	/// 
	//	/// Přípravu dat nechceme měřit a omezovat timeoutem.
	//	/// </summary>
	//	public DbDataLoaderLoadSuppressesChangeTrackerTests()
	//	{
	//		DataLoaderTestDbContext context = new DataLoaderTestDbContext();
	//		context.Database.Initialize(true); // drop&create

	//		// vyrobíme hodně objektů (100 tisíc)
	//		List<Master> masters = new List<Master>(1000);
	//		for (int i = 0; i < 1000; i++)
	//		{
	//			Master master = new Master();
	//			masters.Add(master);
	//			for (int j = 0; j < 100; j++)
	//			{
	//				Child child = new Child();
	//				child.Parent = master;
	//			}
	//		}
	//		context.Set<Master>().AddRange(masters);

	//		context.SaveChanges();
	//	}

	//	/// <summary>
	//	/// Timeout: Limitujeme test na 5 sekund. Při lokálním vývoji běží test BEZ changetrackeru 2 až 3 sekundy, s changetrackerem okolo 25-30 sekund.
	//	/// To platí pro 1000 masterů, každý 100 childů a 10x spuštění DbDataLoader.Load pro každý master.
	//	/// </summary>
	//	[TestMethod]
	//	[Timeout(10000)]
	//	public void DbDataLoaderLoadSuppressesChangeTrackerTest_Load_ShouldBeFastBySuppressingChangeTracker()
	//	{
	//		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
	//		// zaplevelíme identity mapu spoustou objektů
	//		List<Master> masters = dbContext.Set<Master>().Include(m => m.Children).ToList();

	//		// Act
	//		IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
	//		for (int i = 0; i < 10; i++)
	//		{
	//			// dataLoader.Load proběhne 10 (cyklus) * 1000 (počet masterů)
	//			masters.ForEach(master => { dataLoader.Load(master, m => m.Children); });
	//		}

	//		// Assert
	//		// should be finished in time
	//	}
	// }
}
