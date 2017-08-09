using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.Patterns.DataSeeds.Profiles
{
    /// <summary>
    /// Pøedek pro implementaci profilù seedovaných dat.
    /// </summary>
    public abstract class DataSeedProfile : IDataSeedProfile
    {
        /// <summary>
        /// Vrací název profilu. Implementováno tak, že vrací celé jméno typu profilu.
        /// </summary>
        public string ProfileName => this.GetType().FullName;

        /// <summary>
        /// Vrací profily (resp. jejich typy), které musejí být naseedovány pøed tímto profilem.
        /// </summary>
        public virtual IEnumerable<Type> GetPrerequisiteProfiles()
        {
            return Enumerable.Empty<Type>();
        }
    }
}