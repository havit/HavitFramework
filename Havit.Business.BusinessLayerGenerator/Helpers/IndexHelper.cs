using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class IndexHelper
	{
		#region CreateOrUpdate
		/// <summary>
		/// Vytvoří index, pokud neexistuje.
		/// Pokud existuje a je jiný, je původní index odstraněn.
		/// </summary>
		public static void CreateOrUpdate(Table table, Index index, List<string> generatorIndexes)
		{
			generatorIndexes.Add(index.Name);

			bool ok = true;

			Index existingIndex = table.Indexes[index.Name];
			if (existingIndex == null)
			{
				ok = false;
			}
			else
			{
				ok = AreSame(existingIndex, index);
			}

			if (!ok)
			{
				if (existingIndex != null)
				{
					existingIndex.Drop();
				}
				index.Create();
			}
		}
		#endregion

		#region FindByStructure
		/// <summary>
		/// Najde v tabulce existující index ve stejné struktury.
		/// Vrací nalezený index nebo null, není-li nalezen.
		/// </summary>
		public static Index FindByStructure(Table table, Index index)
		{
			foreach (Index tableIndex in table.Indexes)
			{
				if (AreSame(tableIndex, index))
				{
					return tableIndex;
				}
			}
			return null;
		}
		#endregion

		#region AreSame
		/// <summary>
		/// Porovná strukturu indexů. Vrací true, pokud je struktura shodná, jinak false.
		/// </summary>
		private static bool AreSame(Index index1, Index index2)
		{
			bool areSame = true;
			
			if ((index1.IndexedColumns.Count != index2.IndexedColumns.Count) || (index1.IsUnique != index2.IsUnique))
			{
				areSame = false;
			}

			if (areSame)
			{
				List<IndexedColumn> indexColumns1 = AreSame_InitializeByIndexedColumns(index1, item => item.IsIncluded);
				List<IndexedColumn> indexColumns2 = AreSame_InitializeByIndexedColumns(index2, item => item.IsIncluded);

				if (!AreSame(indexColumns1, indexColumns2))
				{
					areSame = false;
				}
			}

			if (areSame)
			{
				List<IndexedColumn> indexColumns1 = AreSame_InitializeByIndexedColumns(index1, item => !item.IsIncluded);
				List<IndexedColumn> indexColumns2 = AreSame_InitializeByIndexedColumns(index2, item => !item.IsIncluded);
				
				if (!AreSame(indexColumns1, indexColumns2))
				{
					areSame = false;
				}
			}

			return areSame;
		}

		private static bool AreSame(List<IndexedColumn> indexColumns1, List<IndexedColumn> indexColumns2)
		{
			bool areSame = true;

			if (indexColumns1.Count != indexColumns2.Count)
			{
				areSame = false;
			}

			if (areSame)
			{
				for (int i = 0; i < indexColumns1.Count; i++)
				{
					if (indexColumns1[i].Name != indexColumns2[i].Name)						
					{
						areSame = false;
					}
				}
			}

			return areSame;
		}

		private static List<IndexedColumn> AreSame_InitializeByIndexedColumns(Index index, Predicate<IndexedColumn> predicate)
		{
			List<IndexedColumn> result = new List<IndexedColumn>();
			foreach (IndexedColumn ic in index.IndexedColumns)
			{
				if (predicate.Invoke(ic))
				{
					result.Add(ic);
				}
			}
			return result;
		}
		#endregion

		#region GetIndexName
		public static string GetIndexName(string prefix, string tableName, params string[] columnNames)
		{
			StringBuilder result = new StringBuilder();
			result.Append(prefix);
			result.Append("_");
			result.Append(tableName);
			foreach (string columnName in columnNames)
			{
				result.Append("_");
				result.Append(columnName);
			}

			if (result.Length > 128)
			{
				string columnNamesJoined = String.Join("", columnNames);
				columnNamesJoined = columnNamesJoined.ToLower();

				MD5 md5 = MD5.Create();
				byte[] hash = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(columnNamesJoined));

				result.Length = 128 - (2 * hash.Length) - 1;
				if (!result.ToString().EndsWith("_"))
				{
					result.Append("_");
				}

				foreach (byte hashByte in hash)
				{
					result.Append(hashByte.ToString("x2"));
				}
			}

			return result.ToString();
		}
		#endregion
	}
}
