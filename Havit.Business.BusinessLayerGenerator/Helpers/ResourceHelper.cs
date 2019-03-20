using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class ResourceHelper
	{
		public static List<ResourceClass> GetResourceClasses(Table resourceClass)
		{
			if (_getResourceClassesResult == null)
			{
				_getResourceClassesResult = new List<ResourceClass>();

				using (SqlCommand command = GetResourceClasses_GetSqlCommand(resourceClass))
				{
					using (SqlDataReader dataReader = ConnectionHelper.GetDataReader(command))
					{
						while (dataReader.Read())
						{
							int id = dataReader.GetInt32(0);
							string className = dataReader.GetString(1);
							string comment = dataReader.IsDBNull(2) ? "" : dataReader.GetString(2);

							_getResourceClassesResult.Add(new ResourceClass
							{
								ID = id,
								ClassName = className,
								Comment = comment
							});
						}
					}
				}
				_getResourceClassesResult.Sort((class1, class2) => String.Compare(class1.ClassName, class2.ClassName, StringComparison.InvariantCultureIgnoreCase));
			}
			return _getResourceClassesResult;
		}
		private static List<ResourceClass> _getResourceClassesResult;

		private static SqlCommand GetResourceClasses_GetSqlCommand(Table resourceClass)
		{
			string idColumn = TableHelper.GetPrimaryKey(resourceClass).Name;
			string classNameColumn = ColumnHelper.FindFirstExistingColumn(resourceClass, "ClassName", "Name", "Nazev");
			string desciptionColumn = ColumnHelper.FindFirstExistingColumn(resourceClass, "Description", "Popis");

			string commandText = String.Format("SELECT [{0}], [{1}], [{2}] FROM [{3}].[{4}]",
				idColumn,
				classNameColumn,
				desciptionColumn ?? "NULL",
				resourceClass.Schema,
				resourceClass.Name);

			return new SqlCommand(commandText);

		}

		public static List<ResourceItem> GetResourceItems(int resourceClassID)
		{
			if (_getResourceItemsResult == null)
			{
				_getResourceItemsResult = new List<ResourceItem>();

				using (SqlCommand command = GetResourceItems_GetSqlCommand())
				{
                    using (SqlDataReader dataReader = ConnectionHelper.GetDataReader(command))
                    {
                        while (dataReader.Read())
                        {
                            int id = dataReader.GetInt32(0);
                            string name = dataReader.GetString(1);
                            int classID = dataReader.GetInt32(2);

                            _getResourceItemsResult.Add(new ResourceItem
                            {
                                ID = id,
                                Name = name,
                                ResourceClassID = classID
                            });
                        }
                    }
				}
			}
			return _getResourceItemsResult.FindAll(item => item.ResourceClassID == resourceClassID);
		}
		private static List<ResourceItem> _getResourceItemsResult;

		private static SqlCommand GetResourceItems_GetSqlCommand()
		{
			Table resourceItem = DatabaseHelper.FindTable("ResourceItem", "dbo");

			string idColumn = TableHelper.GetPrimaryKey(resourceItem).Name;
			string resourceKey = ColumnHelper.FindFirstExistingColumn(resourceItem, "ResourceKey", "Name", "Nazev");
			string resourceClassIDColumn = ColumnHelper.FindFirstExistingColumn(resourceItem, "ResourceClassID");

			string commandText = String.Format("SELECT [{0}], [{1}], [{2}] FROM [{3}].[{4}]",
				idColumn,
				resourceKey,
				resourceClassIDColumn,
				resourceItem.Schema,
				resourceItem.Name);

			return new SqlCommand(commandText);

		}
	}
}
