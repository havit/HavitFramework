using Glimpse.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Glimpse;

/// <summary>
    /// The implementation of <see cref="IInspector"/> for capturing DbConnectorEvent messages.
    /// </summary>
    public class DbConnectorInspector : IInspector
    {
        /// <summary>
        /// Setups the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        /// Executed during the Glimpse.Core.Framework.IGlimpseRuntime.Initialize phase of
        /// system startup. Specifically, with the ASP.NET provider, this is wired to/implemented by the
        /// <c>System.Web.IHttpModule.Init</c> method.
        /// </remarks>
        public void Setup(IInspectorContext context)
        {
		// NOOP
        }
    }
