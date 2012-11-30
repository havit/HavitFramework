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
using Havit.Data.SqlClient;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	/// <summary>
	/// Pomocné metody pro práci s cache.
	/// </summary>
	[System.Diagnostics.Contracts.ContractVerification(false)]
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public static partial class CacheHelper
	{
		#region PreloadCachedObjectsAsync
		/// <summary>
		/// Asynchronně načte do cache všechny cachované objekty.
		/// </summary>
		public static void PreloadCachedObjectsAsync()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(PreloadCachedObjectsAsync_DoWork));
		}
		
		/// <summary>
		/// Asynchronně načte do cache všechny cachované objekty.
		/// </summary>
		private static void PreloadCachedObjectsAsync_DoWork(object state)
		{
			using (new Havit.Business.IdentityMapScope())
			{
				PreloadCachedObjectsAsyncStarting();
				
				try
				{
					Havit.BusinessLayerTest.Role.GetAll();
				}
				catch (SqlException)
				{
				}
				
				PreloadCachedObjectsAsyncCompleted();
			}
		}
		#endregion
		
		static partial void PreloadCachedObjectsAsyncStarting();
		static partial void PreloadCachedObjectsAsyncCompleted();
	}
}
