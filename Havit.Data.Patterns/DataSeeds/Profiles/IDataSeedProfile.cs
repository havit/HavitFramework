using System;
using System.Collections.Generic;

namespace Havit.Data.Patterns.DataSeeds.Profiles;

    /// <summary>
    /// Profil pro seedování dat.
    /// Slouží k možnosti mít v aplikací více možných profilů, například pro testování či ladění různých funkcionalit.
    /// </summary>
    public interface IDataSeedProfile
    {
        /// <summary>
        /// Vrací název profilu.
        /// </summary>
        string ProfileName { get; }

        /// <summary>
        /// Vrací profily (resp. jejich typy), které musejí být naseedovány před tímto profilem.
        /// </summary>
        IEnumerable<Type> GetPrerequisiteProfiles();
    }