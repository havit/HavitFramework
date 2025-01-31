﻿using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

public record ExtendedPropertiesKey
{
	public string Schema { get; }
	public string StoredProcedure { get; }
	public string Table { get; }
	public string Column { get; }

	public ExtendedPropertiesKey(string schema, string table = null, string column = null, string storedProcedure = null)
	{
		// zajišťujeme case insensitivitu
		this.Schema = schema?.ToLower() ?? String.Empty;
		this.Table = table?.ToLower() ?? String.Empty;
		this.Column = column?.ToLower() ?? String.Empty;
		this.StoredProcedure = storedProcedure?.ToLower() ?? String.Empty;
	}

	public static ExtendedPropertiesKey FromTable(Table table)
	{
		return new ExtendedPropertiesKey(schema: table.Schema, table: table.Name);
	}

	public static ExtendedPropertiesKey FromColumn(Column column)
	{
		return new ExtendedPropertiesKey(schema: ((Table)column.Parent).Schema, table: ((Table)column.Parent).Name, column: column.Name);
	}

	public static ExtendedPropertiesKey FromStoredProcedure(StoredProcedure storedProcedure)
	{
		return new ExtendedPropertiesKey(schema: storedProcedure.Schema, storedProcedure: storedProcedure.Name);
	}

	public static ExtendedPropertiesKey FromDatabase()
	{
		return new ExtendedPropertiesKey(schema: null);
	}
}