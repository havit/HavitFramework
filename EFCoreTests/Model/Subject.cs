using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.EFCoreTests.Model
{
	public class Subject
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Address HomeAddress { get; set; }
	}
}
