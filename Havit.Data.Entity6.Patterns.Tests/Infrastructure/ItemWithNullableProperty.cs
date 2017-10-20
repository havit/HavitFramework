using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.Tests
{
    public class ItemWithNullableProperty
    {
        public int Id { get; set; }
        public int? NullableValue { get; set; }
    }
}
