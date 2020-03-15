using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Extensions
{
    /// <summary>
    /// Static extension class for WindsorContainer extensions
    /// </summary>
    public static class WindsorCastleExtensions
    {
        /// <summary>
        /// Registers <see cref="IGoogleAnalyticsMeasurementApiClient"/> and dependent services with transient lifestyle
        /// </summary>
        /// <typeparam name="TConfig">Configuration type to be registered</typeparam>
        /// <param name="windsorContainer">Windsor container</param>
        public static void RegisterGoogleAnalyticMeasurementApiClient<TConfig>(this IWindsorContainer windsorContainer)
            where TConfig : IGoogleAnalyticsMeasurementApiConfiguration
        {
            windsorContainer.Register(
                Component.For<IHttpRequestSender, HttpClientRequestSender>().LifestyleTransient(),
                Component.For<IGoogleAnalyticsMeasurementApiConfiguration, TConfig>().LifestyleTransient(),
                Component.For<IGoogleAnalyticsMeasurementApiClient, GAMeasurementApiClient>().LifestyleTransient());
        }
    }
}
