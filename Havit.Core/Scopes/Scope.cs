using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Havit.Diagnostics.Contracts;

namespace Havit.Scopes
{
	/// <summary>
	/// Thread-specific Scope obalující dosah platnosti určitého objektu (transakce, identity mapy, atp.),
	/// který je následně přístupný přes property metodu GetCurrent (metoda určena k použití v potomcích do veřejné vlastnosti Current).
	/// </summary>
	/// <example>
	/// <code>
	/// using (new Scope&lt;IdentityMap&gt;(new IdentityMap()))
	/// {
    ///		Console.WriteLine(Scope.Current.SomeMethod("outer scope"));
	/// 
    ///		using (new Scope&lt;IdentityMap&gt;(new IdentityMap()))
	///		{
    ///			Console.WriteLine(Scope.Current.SomeMethod("inner scope"));
	///		}
    ///		
	///		Console.WriteLine(Scope.Current.SomeMethod("inner scope"));
	///	}
	/// </code>
	/// </example>
	/// <remarks>
	/// Implementace vycházející z MSDN Magazine článku <a href="http://msdn.microsoft.com/msdnmag/issues/06/09/netmatters/default.aspx">Stephen Toub: Scope&lt;T&gt; and More</a> (již nedostupný).
	/// </remarks>
	/// <typeparam name="T">typ objektu, jehož scope řešíme</typeparam>
	public class Scope<T> : IDisposable
		where T : class
	{
		/// <summary>
		/// Aktuální instance obalovaná scopem.
		/// Určeno pro použití v potomcích pro implementaci statické vlastnosti Current.
		/// </summary>
		/// <param name="scopeRepository">repository pro čtení scope</param>
		protected static T GetCurrent(IScopeRepository<T> scopeRepository)
		{
			Scope<T> scope = scopeRepository.GetCurrentScope();
			return scope != null ? scope.instance : null;
		}

		/// <summary>
		/// Indikuje, zdali již proběhl Dispose třídy.
		/// </summary>
		private bool disposed;

        /// <summary>
        /// Indikuje, zdali je instance scopem vlastněná, tj. máme-li ji na konci scope disposovat.
        /// </summary>
        private readonly bool ownsInstance;

		/// <summary>
		/// Instance, kterou scope obaluje.
		/// </summary>
		private readonly T instance;

		/// <summary>
		/// Nadřazený scope v linked-listu nestovaných scope.
		/// </summary>
		private readonly Scope<T> parent;

		private readonly IScopeRepository<T> scopeRepository;

		/// <summary>
		/// Vytvoří instanci třídy <see cref="Scope{T}"/> kolem instance. Instance bude při disposingu Scope též disposována.
		/// </summary>
		/// <param name="instance">instance, kterou scope obaluje</param>
		/// <param name="scopeRepository">repository pro uložení scope</param>
		protected Scope(T instance, IScopeRepository<T> scopeRepository) : this(instance, scopeRepository, true) { }

		/// <summary>
		/// Vytvoří instanci třídy <see cref="Scope{T}"/> kolem instance.
		/// </summary>
		/// <param name="instance">instance, kterou scope obaluje</param>
		/// <param name="scopeRepository">repository pro uložení scope</param>
		/// <param name="ownsInstance">indikuje, zdali instanci vlastníme, tedy zdali ji máme s koncem scopu disposovat</param>
		protected Scope(T instance, IScopeRepository<T> scopeRepository, bool ownsInstance)
		{
			Contract.Requires<ArgumentNullException>(instance != null, nameof(instance));

			this.instance = instance;
			this.scopeRepository = scopeRepository;
			this.ownsInstance = ownsInstance;

			// linked-list pro nestování scopes
			this.parent = scopeRepository.GetCurrentScope();
			scopeRepository.SetCurrentScope(this);
		}

		/// <summary>
		/// Ukončí scope a disposuje vlastněné instance.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			// no unmanaged resources owned, not needed: Finalize() + GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose. Uvolní objekt reprezentující scope (volám metody Dispose), pokud tento scope implementuje IDisposable.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;

				Debug.Assert(this == scopeRepository.GetCurrentScope(), "Disposed out of order.");
				if (parent == null)
				{
					scopeRepository.RemoveCurrentScope();
				}
				else
				{
					scopeRepository.SetCurrentScope(parent);
				}

				if (ownsInstance)
				{
					IDisposable disposable = instance as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}
}