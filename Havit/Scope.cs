using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Havit
{
	/// <summary>
	/// Thread-specific Scope obalující dosah platnosti určitého objektu (transakce, identity mapy, atp.),
	/// který je následně přístupný přes property <see cref="Current"/>.
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
	/// Implementace vycházející z MSDN Magazine článku <a href="http://msdn.microsoft.com/msdnmag/issues/06/09/netmatters/default.aspx">Stephen Toub: Scope&lt;T&gt; and More</a>.
	/// </remarks>
	/// <typeparam name="T">typ objektu, jehož scope řešíme</typeparam>
	public class Scope<T> : IDisposable
		where T : class
	{
		#region private fields
		/// <summary>
		/// Indikuje, zda-li již proběhl Dispose třídy.
		/// </summary>
		private bool disposed;
		
		/// <summary>
		/// Indikuje, zda-li je instance scopem vlastněná, tj. máme-li ji na konci scope disposovat.
		/// </summary>
		private bool ownsInstance;
		
		/// <summary>
		/// Instance, kterou scope obaluje.
		/// </summary>
		private T instance;
		
		/// <summary>
		/// Nadřazený scope v linked-listu nestovaných scope.
		/// </summary>
		private Scope<T> parent;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří instanci třídy <see cref="Scope{T}"/> kolem instance. Instance bude při disposingu Scope též disposována.
		/// </summary>
		/// <param name="instance">instance, kterou scope obaluje</param>
		public Scope(T instance) : this(instance, true) { }

		/// <summary>
		/// Vytvoří instanci třídy <see cref="Scope{T}"/> kolem instance.
		/// </summary>
		/// <param name="instance">instance, kterou scope obaluje</param>
		/// <param name="ownsInstance">indikuje, zda-li instanci vlastníme, tedy zda-li ji máme s koncem scopu disposovat</param>
		public Scope(T instance, bool ownsInstance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this.instance = instance;
			this.ownsInstance = ownsInstance;

			// linked-list pro nestování scopes
			this.parent = head;
			head = this;
		}
		#endregion

		#region Dispose (IDisposable)
		/// <summary>
		/// Ukončí scope a disposuje vlastněné instance.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Scope()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;

				Debug.Assert(this == head, "Disposed out of order.");
				head = parent;

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
		
		#endregion

		#region private field (thread static)
		/// <summary>
		/// Aktuální konec linked-listu nestovaných scope.
		/// </summary>
		[ThreadStatic]
		private static Scope<T> head;
		#endregion

		#region Current (static)
		/// <summary>
		/// Aktuální instance obalovaná scopem.
		/// </summary>
		public static T Current
		{
			get { return head != null ? head.instance : null; }
		}
		#endregion
	}

}
