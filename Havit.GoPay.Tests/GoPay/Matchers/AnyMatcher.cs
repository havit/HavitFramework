using System.Collections.Generic;
using System.Linq;

namespace Havit.Tests.GoPay.Matchers
{
    /// <summary>
    /// A composite matcher that suceeds if any of it's composed matchers succeed
    /// </summary>
    public class AnyMatcher : IMockedRequestMatcher
    {
        private readonly IEnumerable<IMockedRequestMatcher> matchers;

        /// <summary>
        /// Construcuts a new instnace of AnyMatcher
        /// </summary>
        /// <param name="matchers">The list of matchers to evaluate</param>
        public AnyMatcher(IEnumerable<IMockedRequestMatcher> matchers)
        {
            this.matchers = matchers;
        }

        /// <summary>
        /// Determines whether the implementation matches a given request
        /// </summary>
        /// <param name="message">The request message being evaluated</param>
        /// <returns>true if any of the supplied matchers succeed; false otherwise</returns>
        public bool Matches(System.Net.Http.HttpRequestMessage message)
        {
            return matchers.Any(m => m.Matches(message));
        }
    }
}
