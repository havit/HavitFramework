using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Havit.Collections;

/// <summary>
/// Kolekce objektů třídy SortItem.
/// </summary>
[Serializable]
[DataContract]
public class SortItemCollection : Collection<SortItem>
{
}
