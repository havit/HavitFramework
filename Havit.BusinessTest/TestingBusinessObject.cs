using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business;

namespace Havit.BusinessTest
{
	public class TestingBusinessObject : BusinessObjectBase
	{
		#region Constructors
		public TestingBusinessObject()
			: base()
		{
		}

		public TestingBusinessObject(int id)
			: base(id, ConnectionMode.Connected)
		{
		}
		#endregion

		#region TryLoad_Perform
		protected override bool TryLoad_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion

		#region Save_Perform
		protected override void Save_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion

		#region Delete_Perform
		protected override void Delete_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
	}
}
