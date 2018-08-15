using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Model
{
    /// <summary>
    /// Informace o naposledy spuštěné verzi seedování dat v daném profilu.
    /// </summary>
    public class DataSeedVersion
    {
        /// <summary>
        /// Název profilu
        /// </summary>
        [Key]
        [MaxLength(250)]
        public string ProfileName { get; set; }

        /// <summary>
        /// Naposledy spuštěná verze seedování dat v daném profilu.
        /// </summary>
        [MaxLength]
        public string Version { get; set; }
    }
}
