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
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Пустая сторока подключения!");
            }
            _connectionString = connectionString;
        }

        private string GetConnectionString()
        {
            return _connectionString;
        }

        public IDbConnection CreateSqlConnection()
        {
            return CreateDbConnection(GetConnectionString());
        }
        
        private IDbConnection CreateDbConnection(string connectionString)
        {
            DbConnection connection;
            
            connection = new SqlConnection(); 
            connection.ConnectionString = connectionString;
            
            return connection;
        }
    }
}