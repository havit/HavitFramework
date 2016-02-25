﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest.Resources
{
	/// <summary>
	/// Database resources wrapper.
	/// </summary>
	public partial class DbResources
	{
		#region Private fields
		private Func<System.Globalization.CultureInfo> getCultureInfoMethod;
		#endregion
		
		#region Current
		/// <summary>
		/// Vrací resources pro aktuální culture info.
		/// </summary>
		public static DbResources Current
		{
			get
			{
				return _current;
			}
		}
		private static DbResources _current;
		#endregion
		
		#region Constructor (static)
		/// <summary>
		/// Statický konstruktor. Inicializuje hodnotu pro vlastnost Current.
		/// </summary>
		static DbResources()
		{
			_current = new DbResources(() => System.Globalization.CultureInfo.CurrentUICulture);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		internal DbResources(Func<System.Globalization.CultureInfo> getCultureInfoMethod)
		{
			global::Havit.Diagnostics.Contracts.Contract.Assert(getCultureInfoMethod != null, "getCultureInfoMethod != null");
			
			this.getCultureInfoMethod = getCultureInfoMethod;
		}
		
		/// <summary>
		/// Konstruktor. Vytváří instanci pro resources v dané culture info.
		/// </summary>
		public DbResources(System.Globalization.CultureInfo cultureInfo) : this(() => cultureInfo)
		{
			global::Havit.Diagnostics.Contracts.Contract.Assert(cultureInfo != null, "cultureInfo != null");
		}
		
		/// <summary>
		/// Konstruktor. Vytváří instanci pro resources v daném jazyce.
		/// </summary>
		public DbResources(Havit.BusinessLayerTest.Language language) : this(System.Globalization.CultureInfo.GetCultureInfo(language.Culture))
		{
		}
		#endregion
		
		#region GetString
		/// <summary>
		/// Vrátí resource string.
		/// </summary>
		public static string GetString(string resourceClass, string resourceKey, CultureInfo cultureInfo)
		{
			return GetStringInternal(resourceClass, resourceKey, cultureInfo, false);
		}
		
		private static string GetStringInternal(string resourceClass, string resourceKey, CultureInfo cultureInfo, bool isRecursion)
		{
			string cacheKey = String.Format("DbResources|culture=" + cultureInfo.Name);
			
			Dictionary<string, Dictionary<string, string>> resourceClassesData = (Dictionary<string, Dictionary<string, string>>)HttpRuntime.Cache[cacheKey];
			Dictionary<string, string> resourceKeysData;
			string result = null;
			
			if (resourceClassesData == null)
			{
				lock(_getStringLock)
				{
					resourceClassesData = (Dictionary<string, Dictionary<string, string>>)HttpRuntime.Cache[cacheKey];
					if (resourceClassesData == null)
					{
						resourceClassesData = GetResourceClassesData(cultureInfo);
						HttpRuntime.Cache.Insert(
							cacheKey,
							resourceClassesData,
							new CacheDependency(null, new string[] { Havit.BusinessLayerTest.Resources.ResourceItemLocalization.GetAnySaveCacheDependencyKey() }),
							Cache.NoAbsoluteExpiration,
							Cache.NoSlidingExpiration,
							CacheItemPriority.AboveNormal,
							null /* callback */);
					}
				}
			}
			
			if (resourceClassesData.TryGetValue(resourceClass.ToLowerInvariant(), out resourceKeysData))
			{
				resourceKeysData.TryGetValue(resourceKey.ToLowerInvariant(), out result);
			}
			
			// Pokud se nepodařilo získat hodnotu, zkusíme ji najít ve výchozím jazyce.
			if ((result == null) && (cultureInfo != CultureInfo.InvariantCulture))
			{
				result = GetStringInternal(resourceClass, resourceKey, CultureInfo.InvariantCulture, true);
			}
			
			// v debugu dovolíme spuštění i bez platné hodnoty v resources
			// null hodnota znamená nenalezenou hodnotu v resources a kompilační chybu
#if DEBUG
			if ((result == null) && (!isRecursion))
			{
				result = String.Format("[{0},{1},{2}]", resourceClass, resourceKey, cultureInfo.Name);
			}
#endif
			
			return result;
		}
		private static object _getStringLock = new object();
		#endregion
		
		#region GetResourceClassesData
		/// <summary>
		/// Načte hodnoty lokalizací pro jeden jazyk (resp. cultureInfo).
		/// </summary>
		private static Dictionary<string, Dictionary<string, string>> GetResourceClassesData(CultureInfo cultureInfo)
		{
			Havit.BusinessLayerTest.Language language = Havit.BusinessLayerTest.Language.GetByUICulture(cultureInfo);
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.CommandText = "SELECT _rc.[Name] as ResourceClass, _ri.[ResourceKey] as ResourceKey, _ril.[Value] as Value FROM [dbo].[ResourceClass] _rc INNER JOIN [dbo].[ResourceItem] _ri ON (_rc.[ResourceClassID] = _ri.[ResourceClassID]) INNER JOIN [dbo].[ResourceItemLocalization] _ril ON (_ri.[ResourceItemID] = _ril.[ResourceItemID]) WHERE (_ril.[LanguageID] = @LanguageID) AND (_ril.Value IS NOT NULL);";
			
			DbParameter dbParameterLanguageID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterLanguageID.DbType = DbType.Int32;
			dbParameterLanguageID.Direction = ParameterDirection.Input;
			dbParameterLanguageID.ParameterName = "LanguageID";
			dbParameterLanguageID.Value = language.ID;
			dbCommand.Parameters.Add(dbParameterLanguageID);
			
			List<string[]> resourceClassesData = new List<string[]>();
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					resourceClassesData.Add(new string[] {
						((string)reader["ResourceClass"]).ToLowerInvariant(),
						((string)reader["ResourceKey"]).ToLowerInvariant(),
						(string)reader["Value"] });
				}
			}
			
			return resourceClassesData
				.GroupBy(item => item[0] /* ResourceClass */)
				.ToDictionary(
					item => item.Key /* ResourceClass */,
					group => group.ToDictionary(entry => entry[1] /* ResourceKey */ , entry => entry[2] /* Value */ ));
		}
		#endregion
		
		#region MainResourceClass
		public ResourceClasses.MainResourceClass MainResourceClass
		{
			get
			{
				if (_mainResourceClass == null)
				{
					_mainResourceClass = new ResourceClasses.MainResourceClass(getCultureInfoMethod);
				}
				return _mainResourceClass;
			}
		}
		private ResourceClasses.MainResourceClass _mainResourceClass;
		#endregion
		
		#region ResourceClasses (nested class)
		/// <summary>
		/// Obsahuje třídy pro jednotlivé slovníky resources.
		/// </summary>
		public static partial class ResourceClasses
		{
			#region MainResourceClass (nested class)
			public partial class MainResourceClass
			{
				#region Private fields
				private Func<System.Globalization.CultureInfo> getCultureInfoMethod;
				#endregion
				
				#region Constructor (internal)
				internal MainResourceClass(Func<System.Globalization.CultureInfo> getCultureInfoMethod)
				{
					this.getCultureInfoMethod = getCultureInfoMethod;
				}
				#endregion
				
				#region MainResourceKey
				/// <summary>
				/// Vrací hodnotu z resources pro klíč &quot;MainResourceKey&quot;.
				/// </summary>
				public string MainResourceKey
				{
					get
					{
						return DbResources.GetString("MainResourceClass", "MainResourceKey", getCultureInfoMethod());
					}
				}
				
				#endregion
				
			}
			#endregion
			
		}
		#endregion
		
	}
}
