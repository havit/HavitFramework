using Havit.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Tests.Scopes.Instrastructure;

    internal class TestThreadScope : Scope<object>
    {
        private static readonly IScopeRepository<object> repository = new ThreadScopeRepository<object>();
        private readonly bool suppressDispose;

        public TestThreadScope(object instance, bool suppressDispose = false) : base(instance, repository)
        {
            this.suppressDispose = suppressDispose;
        }

        public static object Current
        {
            get
            {
                return GetCurrent(repository);
            }
        }

        protected override void Dispose(bool disposing)
        {
            // Chceme vypnout hlášení způsobené Debug.Assertem v bázové třídě.
            // Zde, v unit testu, si můžeme dovolit nezavolat bázový dispose.
            if (!suppressDispose)
            {
                base.Dispose(disposing);
            }
        }
    }
