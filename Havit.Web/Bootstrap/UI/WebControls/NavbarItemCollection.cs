﻿using System.Collections;
using System.Web.UI;
using Havit.Web.Bootstrap.UI.WebControls.Infrastructure;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Collection of NavbarItem (and descendants) items.
/// Implements IStateManager to be able to store collection in viewstate.
/// Implements ICollection to be used as a property type in ASPX markup code (ie. Navbar.MenuItems).
/// </summary>
public class NavbarItemCollection : IList<NavbarItem>, IStateManager, ICollection
{
	private readonly List<NavbarItem> items; // menu items
	private readonly List<NavbarViewStateLogEntry> log; // logged changes are used for serializaton to viewstate

	/// <summary>
	/// Return count of collection items.
	/// </summary>
	public int Count
	{
		get { return items.Count; }
	}

	/// <summary>
	/// Always returns false.
	/// </summary>
	public bool IsReadOnly
	{
		get { return false; }
	}

	/// <summary>
	/// Returns item at the index.
	/// </summary>
	public NavbarItem this[int index]
	{
		get
		{
			return items[index];
		}
		set
		{
			items[index] = value;
			if (_isTrackingViewState)
			{
				((IStateManager)value).TrackViewState();
				value.SetDirty();
			}
			this.log.Add(new NavbarViewStateLogEntry(this._isTrackingViewState, LogItemType.Set, index, value == null ? null : value.GetType()));
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public NavbarItemCollection()
	{
		this.items = new List<NavbarItem>();
		log = new List<NavbarViewStateLogEntry>();
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public NavbarItemCollection(IEnumerable<NavbarItem> items)
	{
		this.items = new List<NavbarItem>(items);
		log = new List<NavbarViewStateLogEntry>();
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	public IEnumerator<NavbarItem> GetEnumerator()
	{
		return items.GetEnumerator();
	}

	/// <summary>
	/// Adds item to the end of collection.
	/// </summary>
	public void Add(NavbarItem item)
	{
		this.Insert(items.Count, item);
	}

	/// <summary>
	/// Insert item at the index.
	/// </summary>
	public void Insert(int index, NavbarItem item)
	{
		items.Insert(index, item);

		if (this._isTrackingViewState)
		{
			((IStateManager)item).TrackViewState();
			item.SetDirty();
		}
		this.log.Add(new NavbarViewStateLogEntry(this._isTrackingViewState, LogItemType.Insert, index, item == null ? null : item.GetType()));
	}

	/// <summary>
	/// Clears collection.
	/// </summary>
	public void Clear()
	{
		if (items.Count > 0 || (_isTrackingViewState && this.log.Count > 0))
		{
			items.Clear();

			if (this._isTrackingViewState)
			{
				this.log.Clear();
			}
			this.log.Add(new NavbarViewStateLogEntry(_isTrackingViewState, LogItemType.Clear));
		}
	}

	/// <summary>
	/// Returns index of item in collection.
	/// </summary>
	public int IndexOf(NavbarItem item)
	{
		return items.IndexOf(item);
	}

	/// <summary>
	/// Returns true if item is in collection.
	/// </summary>
	public bool Contains(NavbarItem item)
	{
		return items.Contains(item);
	}

	/// <summary>
	/// Copies items to array.
	/// </summary>
	public void CopyTo(NavbarItem[] array, int arrayIndex)
	{
		items.CopyTo(array, arrayIndex);
	}

	/// <summary>
	/// If item is in collection it is removed.
	/// Returns true, if item was removed.
	/// </summary>
	public bool Remove(NavbarItem item)
	{
		int index = items.IndexOf(item);
		if (index != -1)
		{
			this.RemoveAt(index);
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Remove item at the index.
	/// </summary>
	public void RemoveAt(int index)
	{
		items.RemoveAt(index);
		this.log.Add(new NavbarViewStateLogEntry(this._isTrackingViewState, LogItemType.Remove, index));
	}

	/// <summary>
	/// Set all log items dirty (to be stored in viewstate) including all children.
	/// </summary>
	internal void SetDirty()
	{
		foreach (NavbarViewStateLogEntry item in this.log)
		{
			item.SetDirty();
		}
		for (int i = 0; i < this.Count; i++)
		{
			this[i].SetDirty();
		}
	}

	/// <summary>
	/// Gets an object that can be used to synchronize access to the collection.
	/// </summary>
	object ICollection.SyncRoot
	{
		get { return ((ICollection)items).SyncRoot; }
	}

	/// <summary>
	/// Returns true if access to the collection is synchronized (thread safe); otherwise, false.
	/// </summary>
	bool ICollection.IsSynchronized
	{
		get { return ((ICollection)items).IsSynchronized; }
	}

	/// <summary>
	/// Copies items to array.
	/// </summary>
	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)items).CopyTo(array, index);
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	bool IStateManager.IsTrackingViewState
	{
		get { return _isTrackingViewState; }
	}
	private bool _isTrackingViewState = false;

	void IStateManager.TrackViewState()
	{
		_isTrackingViewState = true;
		foreach (NavbarItem item in items)
		{
			item.TrackViewState();
		}
	}

	object IStateManager.SaveViewState()
	{
		object[] logState = this.log.Select(item => item.SaveViewState()).Where(item => item != null).ToArray();
		if (logState.Length == 0)
		{
			logState = null;
		}

		object[] itemsState = items.Select(item => item.SaveViewState()).ToArray();
		if (itemsState.All(item => item == null))
		{
			itemsState = null;
		}

		if ((logState == null) && (itemsState == null))
		{
			return null;
		}
		else
		{
			return new object[]
			{
				logState,
				itemsState
			};
		}
	}

	void IStateManager.LoadViewState(object state)
	{
		if (state != null)
		{
			object[] savedState = (object[])state;

			object[] logState = null;
			if (savedState[0] != null)
			{
				logState = ((object[])savedState[0]);
			}
			object[] itemsState = (object[])savedState[1];

			if (logState != null)
			{
				foreach (var logItemState in logState)
				{
					NavbarViewStateLogEntry entry = new NavbarViewStateLogEntry(logItemState);

					switch (entry.EntryType)
					{
						case LogItemType.Clear:
							Clear();
							break;

						case LogItemType.Set:
							this[entry.Index.Value] = (NavbarItem)Activator.CreateInstance(entry.ItemType);
							break;

						case LogItemType.Insert:
							Insert(entry.Index.Value, (NavbarItem)Activator.CreateInstance(entry.ItemType));
							break;

						case LogItemType.Remove:
							RemoveAt(entry.Index.Value);
							break;

						default: throw new ApplicationException("Unknown LogItemType.");
					}
				}
			}

			if (itemsState != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					items[i].LoadViewState(itemsState[i]);
				}
			}
		}
		else
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].LoadViewState(null);
			}
		}
	}
}
