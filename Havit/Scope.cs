using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Havit
{
	/// <summary>
	/// Thread-specific Scope obalující dosah platnosti urèitého objektu (transakce, identity mapy, atp.),
	/// který je následnì pøístupný pøes property <see cref="Current"/>.
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
	/// Implementace vycházející z MSDN Magazine èlánku <a href="http://msdn.microsoft.com/msdnmag/issues/06/09/netmatters/default.aspx">Stephen Toub: Scope&lt;T&gt; and More</a>.
	/// </remarks>
	/// <typeparam name="T">typ objektu, jehož scope øešíme</typeparam>
	public class Scope<T> : IDisposable
		where T : class
	{
		#region private fields
		/// <summary>
		/// Indikuje, zda-li již probìhl Dispose tøídy.
		/// </summary>
		private bool disposed;
		
		/// <summary>
		/// Indikuje, zda-li je instance scopem vlastnìná, tj. máme-li ji na konci scope disposovat.
		/// </summary>
		private bool ownsInstance;
		
		/// <summary>
		/// Instance, kterou scope obaluje.
		/// </summary>
		private T instance;
		
		/// <summary>
		/// Nadøazený scope v linked-listu nestovaných scope.
		/// </summary>
		private Scope<T> parent;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci tøídy <see cref="Scope"/> kolem instance. Instance bude pøi disposingu Scope též disposována.
		/// </summary>
		/// <param name="instance">instance, kterou scope obaluje</param>
		public Scope(T instance) : this(instance, true) { }

		/// <summary>
		/// Vytvoøí instanci tøídy <see cref="Scope"/> kolem instance.
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
		/// Ukonèí scope a disposuje vlastnìné instance.
		/// </summary>
		/// <remarks>
		/// ResourceWrapper pattern nepotøebujeme, protože nemáme žádné unmanaged resources, které bychom museli jistit destructorem.
		/// </remarks>
		public virtual void Dispose()
		{
			if (!disposed)
			{
				disposed = true;

				Debug.Assert(this == head, "Disposed out of order.");
				head = parent;

				if (ownsInstance)
				{
					IDisposable disposable = instance as IDisposable;
					if (disposable != null) disposable.Dispose();
				}
			}
		}
		#endregion

		#region private field (static)
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
