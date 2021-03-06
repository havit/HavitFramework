﻿using System;
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
	/// Repository implementující scope jako thread data (Thread.GetData, ThreadSetData).
	/// </summary>
	/// <typeparam name="T">Typ, jehož scope je ukládán do repository.</typeparam>
	public class ThreadScopeRepository<T> : IScopeRepository<T>
		where T : class
	{
		/// <summary>
		/// DataSlot - nepojmenovaný slot, pod kterým jsou ukládány thread data.
		/// </summary>
		private readonly LocalDataStoreSlot threadDataStoreSlot;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ThreadScopeRepository()
		{
			// inicializujeme data slot
			this.threadDataStoreSlot = Thread.AllocateDataSlot();
		}

		/// <summary>
		/// Vrátí hodnotu aktuálního scope.
		/// </summary>
		public Scope<T> GetCurrentScope()
		{
			return (Scope<T>)Thread.GetData(this.threadDataStoreSlot);
		}

		/// <summary>
		/// Nastaví hodnotu aktuálního scope.
		/// </summary>
		public void SetCurrentScope(Scope<T> scope)
		{
			Thread.SetData(this.threadDataStoreSlot, scope);
		}

		/// <summary>
		/// Zruší scope.
		/// </summary>
		public void RemoveCurrentScope()
		{
			Thread.SetData(this.threadDataStoreSlot, null);
		}
	}
}
