using Dapper;
using MediaRecommender.Data.MoviesInternalDb.Dapper;
using MediaRecommender.Data.MoviesInternalDb.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaRecommender.Data.MoviesInternalDb
{
	/// <summary>
	///		The internal database repository.
	/// </summary>
	/// <seealso cref="DapperProviderBase" />
	/// <seealso cref="IInternalDbRepository" />
	public class InternalDbRepository : DapperProviderBase, IInternalDbRepository
    {
		#region Properties

		private readonly ILogger<InternalDbRepository> _logger;

		#endregion

		#region Constructor

		/// <summary>
		///		Initializes a new instance of the <see cref="InternalDbRepository"/> class.
		/// </summary>
		/// <param name="optionsProvider">The options provider.</param>
		public InternalDbRepository(ILogger<InternalDbRepository> logger, IOptions<MoviesInternalDbOptions> optionsProvider) :
            base(optionsProvider.Value.ConnectionString)
        {
			_logger = logger;
        }

		#endregion

		#region Interface Implementation

		/// <summary>
		///		Gets the top 5 successful movies in a city by city name.
		/// </summary>
		/// <param name="cityName">Name of the city.</param>
		/// <returns>The top 5 successful movies in the provided city.</returns>
		/// <exception cref="Exception">Error retrieving successful movies from city [{cityName}].</exception>
		public async Task<IEnumerable<SuccessfulMovie>> GetSuccessfulMoviesByCityName(string cityName)
        {
			string command
				= $@"WITH CTE_MOVIES(MovieId, MovieName, ReleaseDate, SeatsSold) AS(
						SELECT  TOP(5) M.Id      AS MovieId,
								M.OriginalTitle  AS MovieName,
								M.ReleaseDate    AS ReleaseDate,
								SUM(S.SeatsSold) AS SeatsSold
						FROM[dbo].[City] CIT
							INNER JOIN[dbo].[Cinema] CIN
								ON  1 = 1
								AND CIT.Id = CIN.CityId
								AND CIT.Name = '{ cityName }'
							INNER JOIN[dbo].[Room] R
								ON CIN.Id = R.CinemaId
							INNER JOIN[dbo].[Session] S
								ON R.Id = S.RoomId
							INNER JOIN[dbo].Movie M
								ON S.MovieId = M.Id
						GROUP BY M.Id, M.OriginalTitle, M.ReleaseDate
						ORDER BY SUM(S.SeatsSold) DESC
					)
					SELECT MovieName                AS MOVIE_TITLE,
						   SeatsSold			    AS SEATS_SOLD,
						   ReleaseDate			    AS RELEASE_DATE,
						   STRING_AGG(G.Name, ',')  AS GENRES
					FROM CTE_MOVIES M
							INNER JOIN[dbo].[MovieGenre] MG
								ON M.MovieId = MG.MovieId
							INNER JOIN[dbo].[Genre] G
								ON MG.GenreId = G.Id
					GROUP BY M.MovieId, MovieName, SeatsSold, ReleaseDate";

			var cd = new CommandDefinition(command);

			try
			{
				IEnumerable<SuccessfulMovie> queryResult = await ConnectAsync(async c => await c.QueryAsync<SuccessfulMovie>(cd));
				return queryResult;
			}
			catch (Exception ex)
			{
				var message = $"Error retrieving successful movies from city [{cityName}].";

				_logger.LogError(ex, message);
				throw new Exception(message);
			}
        }

        #endregion
    }
}
