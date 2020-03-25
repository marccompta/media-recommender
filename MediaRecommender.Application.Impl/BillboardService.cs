using MediaRecommender.Application.Contracts;
using MediaRecommender.Application.Impl.Mappers;
using MediaRecommender.Entities;
using MediaRecommender.Models.Response;
using MediaRecommender.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaRecommender.Application.Impl
{
    /// <summary>
    ///     A service to handle billboard-related operations.
    /// </summary>
    /// <seealso cref="IBillboardService" />
    public class BillboardService : IBillboardService
    {
        #region Fields

        private readonly IIntelligentBillboardResolver _intelligentBillboardResolver;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="BillboardService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="intelligentBillboardResolver">The intelligent billboard resolver.</param>
        /// <param name="mapper">The domain and application layers mapper.</param>
        public BillboardService(IIntelligentBillboardResolver intelligentBillboardResolver, IMapper mapper)
        {
            _intelligentBillboardResolver = intelligentBillboardResolver;
            _mapper = mapper;
        }

        #endregion

        #region Interface Implementation

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
        public async Task<GetIntelligentBillboardResponse> GetIntelligentBillboardAsync(DateTime? from, int? numberOfWeeks, int? numberOfBigRooms,
            int? numberOfSmallRooms, string city)
        {
            if (!from.HasValue)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (!numberOfWeeks.HasValue)
            {
                throw new ArgumentNullException(nameof(numberOfWeeks));
            }

            if (!numberOfBigRooms.HasValue)
            {
                throw new ArgumentNullException(nameof(numberOfBigRooms));
            }

            if (!numberOfSmallRooms.HasValue)
            {
                throw new ArgumentNullException(nameof(numberOfSmallRooms));
            }

            IEnumerable<IntelligentBillboardRecommendation> response = await _intelligentBillboardResolver.ResolveAsync(from.Value, numberOfWeeks.Value, numberOfBigRooms.Value, numberOfSmallRooms.Value, city);
            var result = CreateGetIntelligentBillboardResponse(response);

            return result;
        }

        #endregion

        #region Private Methods

        private GetIntelligentBillboardResponse CreateGetIntelligentBillboardResponse(IEnumerable<IntelligentBillboardRecommendation> intelligentBillboardRecommendation)
            => new GetIntelligentBillboardResponse
            {
                IntelligentBillboard = intelligentBillboardRecommendation?.Select(b => new IntelligentBillboard
                {
                    WeekNumber = b.WeekNumber,
                    SmallRooms = b.SmallRooms.Select(r => _mapper.DomainToApplicationModel(r)),
                    BigRooms = b.BigRooms.Select(r => _mapper.DomainToApplicationModel(r))
                })
            };

        #endregion
    }
}
