using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.SqlTypes
{
    /// <summary>
    /// Reprezentuje pomocné hodnoty pro databázový typ smalldatetime.
    /// </summary>
    public class SqlSmallDateTime
	{
		#region MinValue
		/// <summary>
        /// Minimální hodnota použitelná pro databázový typ smalldatetime.
        /// </summary>
        public static readonly SqlSmallDateTime MinValue = new SqlSmallDateTime(new DateTime(1900, 1, 1));
		#endregion

		#region MaxValue
		/// <summary>
        /// Maximální hodnota použitelná pro databázový typ smalldatetime.
        /// </summary>
        public static readonly SqlSmallDateTime MaxValue = new SqlSmallDateTime(new DateTime(2079, 6, 6, 23, 59, 00));
		#endregion

		#region Constructor
		private SqlSmallDateTime(DateTime value)
        {
            _value = value;
        }
        #endregion

        #region Value
        /// <summary>
        /// Hodnota reprezentovaná jako DateTime.
        /// </summary>
        public DateTime Value
        {
            get
            {
                return _value;
            }
        }
        private readonly DateTime _value;
        #endregion
    }
}
