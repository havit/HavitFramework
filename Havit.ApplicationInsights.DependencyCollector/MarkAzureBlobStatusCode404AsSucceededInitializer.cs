using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.ApplicationInsights.DependencyCollector
{
    /// <summary>
    /// Ty DependencyTelemetry, které jsou typu Azure blob a zároveň mají result code 404, označí jako succeeded.
    /// Slouží k aplikacím, které používají blob.Exists(), aby requesty s návratovou hodnotu 404 pro neexistující blob nebyly označeny jako failed (success==false).
    /// Bohužel není možné odlišit blob.Exists() od blob.DownloadToStream() apod., dojde tedy k označení všech 404 vůči Azure Blob bez ohledu na typ operace.
    /// </summary>
    public class MarkAzureBlobStatusCode404AsSucceededInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Initializes properties of the specified Microsoft.ApplicationInsights.Channel.ITelemetry object.
        /// </summary>
        public void Initialize(ITelemetry telemetry)
        {
            var dependencyTelemetry = telemetry as DependencyTelemetry;
            if (dependencyTelemetry == null)
            {
                return;
            }

            if (IsAzureBlobRequest(dependencyTelemetry) && Is404Result(dependencyTelemetry))
            {
                dependencyTelemetry.Success = true;
                dependencyTelemetry.Context.Properties["AzureBlobStatusCode404AsSuccess"] = "true";
            }
        }

        private bool Is404Result(DependencyTelemetry telemetry)
        {
            return telemetry.ResultCode == "404";
        }

        private bool IsAzureBlobRequest(DependencyTelemetry telemetry)
        {
            return telemetry.Type == "Azure blob";
        }
    }
    
}
