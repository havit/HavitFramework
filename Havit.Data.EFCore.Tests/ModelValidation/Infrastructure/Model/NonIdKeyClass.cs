using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
    public class NonIdKeyClass
    {
		[Key]
		public int Key { get; set; }
    }
}
