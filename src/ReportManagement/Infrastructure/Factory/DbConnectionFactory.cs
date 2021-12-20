using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Infrastructure.Interfaces;

namespace Infrastructure.Factory
{
    public class DbConnectionFactory : IDBConnectionFactory
    {
        private readonly string _connectionString;
        private IDbConnection? _dbConnection;
        private readonly object _locker = new();
        
        public DbConnectionFactory()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString;
            
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Пустая сторока подключения!");
            }
            
            _connectionString = connectionString;
        }

        public IDbConnection CreateSqlConnection()
        {
            if (_dbConnection == null)
            {
                lock (_locker)
                {
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlConnection(_connectionString);
                        _dbConnection.Open();
                    }
                }
            }

            return _dbConnection;
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }
    }
}