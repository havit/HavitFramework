using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
	public static class ScriptCacheHelper
	{
		#region FunctionCache
		/// <summary>
		/// Cache pro klientské skripty. Klíèem je skript a parametry funkce, hodnotou je název funkce,
		/// ve které je skript registrován.
		/// Cache je uložena v HttpContextu.
		/// </summary>
		private static Dictionary<string, string> FunctionCache
		{
			get
			{
				Dictionary<string, string> result = (Dictionary<string, string>)HttpContext.Current.Items[typeof(ScriptCacheHelper)];

				if (result == null)
				{
					// pokud cache ještì není, vytvoøíme ji a vrátíme
					// žádné zámky (lock { ... }) nejsou potøeba, jsme stále v jednom HttpContextu
					result = new Dictionary<string, string>();
					HttpContext.Current.Items[typeof(ScriptCacheHelper)] = result;
				}

				return result;
			}
		}
		#endregion

		#region AddFunctionToCache
		/// <summary>
		/// Pøidá klientský skript do cache.
		/// </summary>
		/// <param name="functionName">Název funkce, ve které je skript registrován.</param>
		/// <param name="code">Klientský skript.</param>
		public static void AddFunctionToCache(string functionName, string[] parameters, string code)
		{
			FunctionCache.Add(GetCacheKey(parameters, code), functionName);
		}		
		#endregion

		#region GetFunctionNameFromCache
		/// <summary>
		/// Nalezne název funkce, ve které je klientský skript registrován.
		/// </summary>
		/// <param name="code">Klientský skript.</param>
		/// <returns>Nalezne název funkce, ve které je klientský skript 
		/// registrován. Pokud skript není registrován, vrátí null.</returns>
		public static string GetFunctionNameFromCache(string[] parameters, string code)
		{
			string result;
			if (FunctionCache.TryGetValue(GetCacheKey(parameters, code), out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}
		#endregion

#warning Comment
		private static string GetCacheKey(string[] parameters, string code)
		{
			if (parameters == null)
			{
				return code;
			}
			else
			{
				return String.Join("|", parameters) + "|" + code;
			}
		}
	}
}
