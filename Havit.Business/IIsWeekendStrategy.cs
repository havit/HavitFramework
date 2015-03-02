using System;

namespace Havit.Business
{
	/// <summary>
	/// Strategie pro určení, zda je daný den víkendem.
	/// </summary>
	public interface IIsWeekendStrategy
	{
		/// <summary>
		/// Vrací true, pokud na daný den připadá víkend.
		/// </summary>
		bool IsWeekend(DateTime date);
	}
}