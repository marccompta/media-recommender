using MediaRecommender.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Resolvers
{
    /// <summary>
    ///     Resolves the intelligent billboard.
    /// </summary>
    public interface IIntelligentBillboardResolver
    {
        /// <summary>
        ///     Resolves the intelligent billboard based on a set of provided parameters asynchronously.
        /// </summary>
        /// <param name="from">The first day of the first week of the set of weeks for which the intelligent billboard is to be calculated.</param>
        /// <param name="numberOfWeeks">The number of weeks a billboard is to be calculated.</param>
        /// <param name="numberOfBigRooms">The number of big rooms.</param>
        /// <param name="numberOfSmallRooms">The number of small rooms.</param>
        /// <param name="city">The city location.</param>
        /// <returns>The intelligent billboard of recommendations for the set of weeks that corresponds to the provided parameters.</returns>
        Task<IEnumerable<IntelligentBillboardRecommendation>> ResolveAsync(DateTime from, int numberOfWeeks, int numberOfBigRooms,
            int numberOfSmallRooms, string city);
    }
}
