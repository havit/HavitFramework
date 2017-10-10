using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Scopes
{
	/// <summary>
	/// Interface pro třídy implementující scope repository - repository umožňující uložit a načítst hodnotu do scope.
	/// </summary>
	/// <typeparam name="T">Typ, jehož scope je ukládán do repository.</typeparam>
	public interface IScopeRepository<T>
		where T : class
	{
		/// <summary>
		/// Vrátí hodnotu aktuálního scope.
		/// </summary>
		Scope<T> GetCurrentScope();

		/// <summary>
		/// Nastaví hodnotu aktuálního scope.
		/// </summary>
		void SetCurrentScope(Scope<T> scope);

		/// <summary>
		/// Zruší scope.
		/// </summary>
		void RemoveCurrentScope();

	}
}
