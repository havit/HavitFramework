using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Havit.Scopes
{
	/// <summary>
	/// Repository implementující scope pomocí AsyncLocal&lt;T&gt;.
	/// </summary>
	/// <typeparam name="T">Typ, jehož scope je ukládán do repository.</typeparam>
	public class AsyncLocalScopeRepository<T> : IScopeRepository<T>
		where T : class
	{
		private readonly AsyncLocal<Scope<T>> storage;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AsyncLocalScopeRepository()
		{
			// inicializujeme data slot
			this.storage = new AsyncLocal<Scope<T>>();
		}

		/// <summary>
		/// Vrátí hodnotu aktuálního scope.
		/// </summary>
		public Scope<T> GetCurrentScope()
		{
			return storage.Value;
		}

		/// <summary>
		/// Nastaví hodnotu aktuálního scope.
		/// </summary>
		public void SetCurrentScope(Scope<T> scope)
		{
			storage.Value = scope;
		}

		/// <summary>
		/// Zruší scope.
		/// </summary>
		public void RemoveCurrentScope()
		{
			storage.Value = null;
		}
	}
}
