namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Interface pro IDbContext factory.
	/// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DbContext je registrován scoped, proto se této factory popsaná issue týká. Je třeba na každém místě, kde je factory použita, ověřit dopady.
	/// </remarks>
	public interface IDbContextFactory
	{
		/// <summary>
		/// Creates/resolves the service.
		/// </summary>
		/// <returns>Service created/resolved.</returns>
		IDbContext CreateService();

		/// <summary>
		/// Releases the service.
		/// </summary>
		/// <param name="service">Service to be released.</param>
		void ReleaseService(IDbContext service);
	}
}