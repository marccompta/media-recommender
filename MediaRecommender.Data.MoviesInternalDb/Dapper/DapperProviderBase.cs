using Dapper;
using MediaRecommender.Data.MoviesInternalDb.Database;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaRecommender.Data.MoviesInternalDb.Dapper
{
    public class DapperProviderBase
    {
        #region Fields

        private readonly string _connectionString;

        #endregion

        #region Constructor

        public DapperProviderBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Public Methods

        public async Task<T> ConnectAsync<T>(Func<IDbConnection, Task<T>> f)
        {
            ConfigureDapperColumnMapping<T>();
            var sqlDatabaseConnectionProvider = new SqlDatabaseConnectionProvider();
            return await sqlDatabaseConnectionProvider.ExecuteDbWorkAsync(f, _connectionString);
        }

        #endregion

        #region Private Methods

        private void ConfigureDapperColumnMapping<T>()
        {
            ConfigureDapperColumnMapping(typeof(T));
        }

        private void ConfigureDapperColumnMapping(Type propertyType)
        {
            Type[] t = { propertyType };

            if (propertyType.IsGenericType)
            {
                t = propertyType.GetGenericArguments();
            }

            Func<Type, string, PropertyInfo> mapping = (type, columnName) =>
                type.GetProperties().FirstOrDefault(prop => DapperConfigurationHelpers.GetColumnAttributeValue(prop) == columnName);

            foreach (var t1 in t)
            {
                SqlMapper.SetTypeMap(t1, new CustomPropertyTypeMap(t1, mapping));

                foreach (var property in t1.GetProperties(BindingFlags.Instance
                                                          | BindingFlags.NonPublic
                                                          | BindingFlags.Public))
                {
                    if (property.PropertyType.IsValueType == false)
                    {
                        ConfigureDapperColumnMapping(property.PropertyType);
                    }
                }
            }
        }

        #endregion
    }
}
