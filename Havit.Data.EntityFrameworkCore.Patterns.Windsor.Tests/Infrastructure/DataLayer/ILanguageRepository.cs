using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer
{
	public interface ILanguageRepository : IRepository<Language>
	{
	}
}
