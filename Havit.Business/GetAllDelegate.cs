using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Business;

/// <summary>
/// Delegát na metodu GetAll.
/// </summary>
/// <returns>Vrací všechny (nesmazané) objekty dané třídy.</returns>
public delegate ICollection GetAllDelegate();
