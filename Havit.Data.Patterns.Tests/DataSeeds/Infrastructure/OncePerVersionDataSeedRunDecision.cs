using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataSeeds;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure
{
	public class OncePerVersionDataSeedRunDecision : OncePerVersionDataSeedRunDecisionBase
	{
		public OncePerVersionDataSeedRunDecision(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister) : base(dataSeedRunDecisionStatePersister)
		{
		}

		protected override Assembly GetAssembly()
		{
			return typeof(OncePerVersionDataSeedRunDecision).Assembly;
		}
	}
}
