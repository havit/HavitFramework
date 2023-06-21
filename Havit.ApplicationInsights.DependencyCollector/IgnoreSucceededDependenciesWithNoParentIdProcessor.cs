using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.ApplicationInsights.DependencyCollector;

    /// <summary>
    /// Ignoruje (neposílá) úspěšně dokončené DependencyTelemetry, které nemají přiřazeno ParentId.
    /// </summary>
    /// <remarks>
    /// Použití je určeno pro Hangfire Server, kde tímto procesorem můžeme zajistit ignorování infrastrukturních dependencies Hangfire.
    /// Dependencies v rámci spuštěných jobů mají ParentId, pokud je použit <c>Havit.Hangfire.Extensions.Filters.ApplicationInsightAttribute"</c>.
    /// </remarks>
    public class IgnoreSucceededDependenciesWithNoParentIdProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        /// <summary>
        /// Konstrutor.
        /// </summary>
        public IgnoreSucceededDependenciesWithNoParentIdProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        /// <inheritdoc />
        public void Process(ITelemetry item)
        {
            if ((item is DependencyTelemetry dependencyTelemetry) && (dependencyTelemetry.Success.GetValueOrDefault(false) /* Success je nullable */) && String.IsNullOrEmpty(item.Context.Operation.ParentId))
            {
                return; // ignorujeme položku
            }

            this.Next.Process(item);
        }
    }
    
