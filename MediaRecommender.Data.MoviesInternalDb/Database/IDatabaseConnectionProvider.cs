using System;
using System.Data;
using System.Threading.Tasks;

namespace MediaRecommender.Data.MoviesInternalDb.Database
{
    public interface IDatabaseConnectionProvider
    {
        #region Public Methods

        Task<T> ExecuteDbWorkAsync<T>(Func<IDbConnection, Task<T>> work, string connectionString);

        Task ExecuteDbWorkAsync(Func<IDbConnection, Task> work, string connectionString);

        #endregion
    }
}
