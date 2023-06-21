using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

    public class ProfileWithPrerequisite : DataSeedProfile
    {
        public override IEnumerable<Type> GetPrerequisiteProfiles()
        {
            yield return typeof(DefaultProfile);
        }
    }
