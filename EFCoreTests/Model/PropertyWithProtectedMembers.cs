using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Havit.EFCoreTests.Model
{
	public class PropertyWithProtectedMembers
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public string ProtectedSetterValue { get; protected set; }

		protected string ProtectedValue { get; set; }

		public void SetProtectedSetterValue(string value)
		{
			ProtectedSetterValue = value;
		}

		public void SetProtectedValue(string value)
		{
			ProtectedValue = value;
		}
	}
}
