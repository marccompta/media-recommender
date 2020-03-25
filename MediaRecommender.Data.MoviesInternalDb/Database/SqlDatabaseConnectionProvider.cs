using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace MediaRecommender.Data.MoviesInternalDb.Database
{
    public class SqlDatabaseConnectionProvider : IDatabaseConnectionProvider, IDisposable
    {
        #region Fields

        private static readonly Hashtable _semaphores = new Hashtable();

        #endregion

        #region Public Methods

        public async Task<T> ExecuteDbWorkAsync<T>(Func<IDbConnection, Task<T>> work, string connectionString)
        {
            SemaphoreSlim semaphore = null;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("The connection string cannot be null or empty", nameof(connectionString));
            }

            var result = default(T);
            try
            {
                semaphore = GetSemaphore(connectionString);

                await semaphore.WaitAsync();

                using (var connection = await GetOpenDatabaseConnectionAsync(connectionString))
                {
                    result = await work(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed executing db work.", ex);
            }
            finally
            {
                if (semaphore != null)
                {
                    semaphore.Release();
                }
            }

            return result;
        }

        public async Task ExecuteDbWorkAsync(Func<IDbConnection, Task> work, string connectionString)
        {
            SemaphoreSlim semaphore = null;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("The connection string cannot be null or empty", nameof(connectionString));
            }

            try
            {
                semaphore = GetSemaphore(connectionString);

                await semaphore.WaitAsync();

                // Execute code protected by the semaphore.
                using (var connection = await GetOpenDatabaseConnectionAsync(connectionString))
                {
                    await work(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed executing db work.", ex);
            }
            finally
            {
                semaphore?.Release();
            }
        }

        #endregion

        #region Private Methods

        private SemaphoreSlim GetSemaphore(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            SemaphoreSlim connectionSemaphore = null;

            if (_semaphores.ContainsKey(connectionString))
            {
                connectionSemaphore = _semaphores[connectionString] as SemaphoreSlim;
            }

            if (connectionSemaphore == null)
            {
                int maxConcurrency = Math.Max(connectionStringBuilder.MaxPoolSize - 1, 1);
                connectionSemaphore = new SemaphoreSlim(maxConcurrency);
                _semaphores[connectionString] = connectionSemaphore;
            }

            return connectionSemaphore;
        }

        private async Task<IDbConnection> GetOpenDatabaseConnectionAsync(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("The connection string cannot be null or empty", "connectionString");
            }

            try
            {
                var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed creating and opening a new sql connection", ex);
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Semaphores not disposed so we want them to be static and have the lifetime of the app domain.
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
