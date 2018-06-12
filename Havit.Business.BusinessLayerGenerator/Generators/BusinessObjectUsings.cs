using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Writers;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectUsings
	{
		#region WriteUsings
		/// <summary>
		/// Zapíše usings na všechny možné potřebné namespace.
		/// </summary>
		public static void WriteUsings(CodeWriter writer)
		{
			writer.WriteLine("using System;");
			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine("using System.Collections.Specialized;");
			writer.WriteLine("using System.Data;");
			writer.WriteLine("using System.Data.Common;");
			writer.WriteLine("using System.Data.SqlClient;");
			writer.WriteLine("using System.Data.SqlTypes;");
			writer.WriteLine("using System.Globalization;");
			writer.WriteLine("using System.Linq;");
			writer.WriteLine("using System.Text;");
			writer.WriteLine("using System.Threading;");
			writer.WriteLine("using System.Web;");
			writer.WriteLine("using System.Web.Caching;");
			writer.WriteLine("using System.Xml;");
			writer.WriteLine("using Havit.Business;");
			writer.WriteLine("using Havit.Business.Query;");
			writer.WriteLine("using Havit.Collections;");
			writer.WriteLine("using Havit.Data;");
			writer.WriteLine("using Havit.Data.SqlServer;");
			writer.WriteLine("using Havit.Data.SqlTypes;");
			writer.WriteLine();

		}
		#endregion

		#region RemoveObsoleteUsings
		public static void RemoveObsoleteUsings(string fileName, TfsClient.SourceControlClient sourceControlClient)
		{
			List<string> lines = File.ReadLines(FileHelper.ResolvePath(fileName)).ToList();
			int linesRemoved = lines.RemoveAll(line => line.StartsWith("using Havit.Data.SqlClient;"));

			if (linesRemoved > 0)
			{
				CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName), sourceControlClient);
				writer.WriteRawLines(lines);
				writer.Save();
			}
		}
		#endregion

	}
}
