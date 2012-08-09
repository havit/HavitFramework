using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;

namespace Havit.BusinessTest
{
	public class TestingBusinessObject : BusinessObjectBase
	{
		public TestingBusinessObject()
			: base()
		{
		}

		public TestingBusinessObject(int id)
			: base(id)
		{
		}

		protected override void Load_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override void Save_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override void Delete_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
