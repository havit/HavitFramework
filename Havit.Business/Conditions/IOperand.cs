using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business.Conditions
{
	public interface IOperand
	{
		string GetCommandValue(DbCommand command);
	}
}
