using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model
{
    public class NonIdKeyClass
    {
		[Key]
		public int Key { get; set; }
    }
}
