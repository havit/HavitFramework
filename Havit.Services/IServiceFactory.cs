namespace Havit.Services
{
	/// <summary>
	/// Generic interface for service factories.
	/// Intended to be used with IoC/DI containers, e.g. Castle Windsor Typed Factories facility:
	/// <code>
	/// 	container.AddFacility&lt;TypedFactoryFacility&gt;();
	///		container.Register(Component.For(typeof(IServiceFactory&lt;IServiceFactory&lt;&gt;&gt;)).AsFactory());
	/// </code>
	/// Be careful with ASP.NET Core where ServiceFactory do not respect lifestyle, when used "per-web-request" lifestyle, Service factory does return
	/// see https://github.com/volosoft/castle-windsor-ms-adapter/issues/32
	/// </summary>
	/// <typeparam name="T">type of service to be managed by the factory</typeparam>
	public interface IServiceFactory<T>
		where T : class
	{
		/// <summary>
		/// Creates/resolves the service.
		/// </summary>
		/// <returns>Service created/resolved.</returns>
		T CreateService();

		/// <summary>
		/// Releases the service.
		/// </summary>
		/// <param name="service">Service to be released.</param>
		void ReleaseService(T service);
	}
}