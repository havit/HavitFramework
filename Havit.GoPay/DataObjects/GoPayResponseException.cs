using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoPay.DataObjects
{
	public class GoPayResponseException : ApplicationException
	{
		public GoPayResponseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
