﻿#if NETFRAMEWORK
using System.Collections.Concurrent;
using System.Web;
using Havit.Scopes;

namespace Havit.Business.Scopes;

/// <summary>
/// Repository implementující scope jako HttpContext.
/// Pro případy, kdy není HttpContext k dispozici (použití v Tasks, apod.) používá ThreadScopeRepository.
/// </summary>
/// <typeparam name="T">Typ, jehož scope je ukládán do repository.</typeparam>
public class WebApplicationScopeRepository<T> : IScopeRepository<T>
	where T : class
{
	/// <summary>
	/// Úložiště scopes k HttpContextům.
	/// </summary>
	private readonly ConcurrentDictionary<HttpContext, Scope<T>> _data = new ConcurrentDictionary<HttpContext, Scope<T>>();

	/// <summary>
	/// ThreadScopeRepository pro případy scopes mimo HttpContext.
	/// </summary>		
	private readonly ThreadScopeRepository<T> threadScopeRepository = new ThreadScopeRepository<T>();

	/// <summary>
	/// Vrátí hodnotu aktuálního scope.
	/// </summary>
	public Scope<T> GetCurrentScope()
	{
		HttpContext context = HttpContext.Current;
		if (context != null)
		{
			Scope<T> result;
			if (_data.TryGetValue(context, out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}
		else
		{
			return threadScopeRepository.GetCurrentScope();
		}
	}

	/// <summary>
	/// Nastaví hodnotu aktuálního scope.
	/// </summary>
	public void SetCurrentScope(Scope<T> value)
	{
		HttpContext context = HttpContext.Current;
		if (context != null)
		{
			_data.AddOrUpdate(context, (missingKey) => value, (existingKey, existingValue) => value);
		}
		else
		{
			threadScopeRepository.SetCurrentScope(value);
		}
	}

	/// <summary>
	/// Zruší scope.
	/// </summary>
	public void RemoveCurrentScope()
	{
		HttpContext context = HttpContext.Current;
		if (context != null)
		{
			Scope<T> value;
			_data.TryRemove(context, out value);
		}
		else
		{
			threadScopeRepository.RemoveCurrentScope();
		}
	}
}
#endif