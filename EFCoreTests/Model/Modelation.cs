using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model;

public class Modelation
{
	public int Id { get; set; }

	public BusinessCase BusinessCase { get; set; }
	public int BusinessCaseId { get; set; }
}
