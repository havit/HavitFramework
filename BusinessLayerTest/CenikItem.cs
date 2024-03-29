﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest;

/// <summary>
/// Test pro money.
/// </summary>
public partial class CenikItem : CenikItemBase
{
	public new Decimal CenaAmount
	{
		get
		{
			return base.CenaAmount;
		}
		set
		{
			base.CenaAmount = value;
		}
	}

	public new Currency CenaCurrency
	{
		get
		{
			return base.CenaCurrency;
		}
		set
		{
			base.CenaCurrency = value;
		}
	}
}
