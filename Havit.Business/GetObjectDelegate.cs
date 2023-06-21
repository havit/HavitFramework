using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business;

/// <summary>
/// Delegát na metodu GetObject.
/// </summary>
/// <param name="objectID">ID objektu, který se má metodou vrátit.</param>
/// <returns>Business objekt na základě ID.</returns>
public delegate BusinessObjectBase GetObjectDelegate(int objectID);
