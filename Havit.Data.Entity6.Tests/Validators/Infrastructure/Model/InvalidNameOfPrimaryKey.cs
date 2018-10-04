using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
    public class InvalidNameOfPrimaryKey
    {
        [Key]
        public int PrimaryKey { get; set; }

        public int Id { get; set; }

        public int ExternalId { get; set; }

    }
}
