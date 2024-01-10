using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class WithComputedColumns
{
	public int Id { get; set; }

	public string Computed { get; set; }

	public string ComputedStored { get; set; }
}
