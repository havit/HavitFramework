using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer
{
	public interface ILanguageRepository : IRepository<Language>
	{
	}
}
