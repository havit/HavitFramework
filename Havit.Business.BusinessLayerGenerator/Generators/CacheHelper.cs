using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class CacheHelper
	{
		public static string WriteCacheOptionsCode(CodeWriter writer, Table table)
		{
			int? absoluteExpirationSeconds = TableHelper.GetCacheAbsoluteExpirationSeconds(table);
			int? slidingExpirationSeconds = TableHelper.GetCacheSlidingExpirationSeconds(table);
			string cachePriority = TableHelper.GetCachePriority(table);

			List<string> lines = new List<string>();
			if (absoluteExpirationSeconds != null)
			{
				lines.Add($"AbsoluteExpiration = new TimeSpan(0, 0, {absoluteExpirationSeconds.Value})");
			}
			if (slidingExpirationSeconds != null)
			{
				lines.Add($"SlidingExpiration = new TimeSpan(0, 0, {slidingExpirationSeconds.Value})");
			}
			if (!String.IsNullOrEmpty(cachePriority))
			{
				lines.Add($"Priority = Havit.Services.Caching.CacheItemPriority.{cachePriority}");
			}

			if (lines.Count > 0)
			{
				writer.WriteLine("Havit.Services.Caching.CacheOptions options = new Havit.Services.Caching.CacheOptions");
				writer.WriteLine("{");
				for (int i = 0; i < lines.Count; i++)
				{
					string line = lines[i];
					if (i < lines.Count - 1)
					{
						line = line + ",";
					}					
					writer.WriteLine(line);
				}
				writer.Unindent();
				writer.WriteLine("};");
				return "options";
			}
			return null;
		}

		public static string GetCacheKeyCore(Table table)
		{
			if (table == _getCacheKeyCoreLastTable)
			{
				return _getCacheKeyCoreLastResult;
			}

			string shortName = ClassHelper.GetClassName(table);
			string longName = ClassHelper.GetClassFullName(table, false);

			// pokud je stejný název třídy ve více namespaces, pak použijeme název vč. namespace, jinak jen samotný název třídy
			var duplicates = DatabaseHelper.GetWorkingTables().Select(dbTable => ClassHelper.GetClassName(dbTable)).Where(name => name == shortName).Count();
			string result = duplicates == 1 ? shortName : longName;

			_getCacheKeyCoreLastTable = table;
			_getCacheKeyCoreLastResult = result;

			return result;
		}
		private static Table _getCacheKeyCoreLastTable;
		private static string _getCacheKeyCoreLastResult;

	}
}
