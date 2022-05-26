
using BuildingBlocks.Repository.Interface;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BuildingBlocks.Repository
{
    public class SqlServerConnectionFactory 
    {
        private readonly IOptions<DatabaseConnection> _databaseOptions;
        private readonly IRetryPolicy _retryPolicy;

        public SqlServerConnectionFactory(IOptions<DatabaseConnection> databaseOptions)
        {
            _databaseOptions = databaseOptions;
            _retryPolicy = new DatabaseCommunicationRetryPolicy();
        }

    
        protected async Task<T> Connection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                await using var connection = new SqliteConnection(_databaseOptions.Value.Connection);
                await _retryPolicy.ExecuteAsync(() => connection.OpenAsync());
                 
                return await getData(connection);
            }
            catch( System. Exception ex)
            {
                throw;
            }
           
        }
    }
}
