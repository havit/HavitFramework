using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Havit.Collections;

/// <summary>
/// Collection of objects of the SortItem class.
/// </summary>
[Serializable]
[DataContract]
public class SortItemCollection : Collection<SortItem>
{
}
