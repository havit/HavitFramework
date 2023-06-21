using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Mapping;

public interface IEntityMappingReader
{
	List<MappedEntity> GetMappedEntities(DbContext dbContext);
}
