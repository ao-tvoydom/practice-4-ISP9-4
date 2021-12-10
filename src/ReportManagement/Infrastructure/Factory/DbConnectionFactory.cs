using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Infrastructure.Interfaces;

namespace Infrastructure.Factory
{
    public class DbConnectionFactory : IDBConnectionFactory
    {
        private readonly string _connectionString;
        
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        private string GetConnectionString()
        {
            if (_connectionString == null)
            {
                throw new Exception("Connection string was not found");
            }

            return _connectionString;
        }

        public IDbConnection CreateSqlServerConnection()
        {
            return CreateDbConnection(GetConnectionString());
        }
        
        private IDbConnection CreateDbConnection(string connectionString)
        {
            DbConnection connection = null;

            if (connectionString != null)
            {
                connection = new SqlConnection();
                connection.ConnectionString = connectionString;
            }
            return connection;
        }
    }
}