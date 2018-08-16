using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services
{
	public class ServiceFactoryDisposableWrapper<TService> : IDisposable
		where TService : class
	{
		private readonly IServiceFactory<TService> serviceFactory;

		internal ServiceFactoryDisposableWrapper(IServiceFactory<TService> serviceFactory)
		{
			this.serviceFactory = serviceFactory;
			Service = serviceFactory.CreateService();
		}

		public TService Service { get; }

		public static implicit operator TService(ServiceFactoryDisposableWrapper<TService> wrapper) => wrapper.Service;

		public void Dispose()
		{
			serviceFactory.ReleaseService(Service);
		}
	}
}
