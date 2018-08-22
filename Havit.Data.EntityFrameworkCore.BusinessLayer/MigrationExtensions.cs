using System;
using System.IO;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
    public static class MigrationExtensions
    {
        public static void SqlResource(this MigrationBuilder migrationBuilder, string resourceName, bool suppressTransaction = false)
        {
			Contract.Requires<ArgumentNullException>(resourceName != null);

            using (var textStream = new StreamReader(Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName)))
            {
                string sql = textStream.ReadToEnd();
	            migrationBuilder.Sql(sql, suppressTransaction);
            }
        }
    }
}