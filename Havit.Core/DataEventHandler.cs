namespace Havit;

/// <summary>
/// Delegate for an event passing between the caller and the called data object.
/// </summary>
/// <typeparam name="T">Type of the passed object.</typeparam>
/// <param name="sender">The caller.</param>
/// <param name="e">Event arguments passing the data object.</param>
public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> e);
