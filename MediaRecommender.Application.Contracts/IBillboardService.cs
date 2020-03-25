using MediaRecommender.Models.Response;
using System;
using System.Threading.Tasks;

namespace MediaRecommender.Application.Contracts
{
    /// <summary>
    ///     A service to handle billboard-related operations.
    /// </summary>
    public interface IBillboardService
    {
        /// <summary>
        ///     Gets the intelligent billboard based on a period of time, a number of big and small rooms, and a city location.
        ///     All the movies will be different from one week to the next one.
        /// </summary>
        /// <param name="from">The date of the first day of the first week of the period.</param>
        /// <param name="numberOfWeeks">The number of consecutive weeks.</param>
        /// <param name="numberOfBigRooms">The number of big rooms.</param>
        /// <param name="numberOfSmallRooms">The number of small rooms.</param>
        /// <param name="city">The city location.</param>
        /// <returns>An intelligent billboard generated according to the provided parameters.</returns>
        Task<GetIntelligentBillboardResponse> GetIntelligentBillboardAsync(DateTime? from, int? numberOfWeeks, int? numberOfBigRooms, 
            int? numberOfSmallRooms, string city);
    }
}
