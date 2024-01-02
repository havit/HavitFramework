using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit;

/// <summary>
/// Event arguments for the DataEventHandler, through which the event invocation and handling can pass data (in both directions).
/// </summary>
/// <typeparam name="T">The type of the passed data.</typeparam>
public class DataEventArgs<T> : EventArgs
{
	/// <summary>
	/// The data object that is passed between the caller and the callee.
	/// </summary>
	public T Data { get; set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public DataEventArgs()
	{

	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public DataEventArgs(T data)
	{
		this.Data = data;
	}
}
