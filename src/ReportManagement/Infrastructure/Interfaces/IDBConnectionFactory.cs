using System;
using System.Data;

namespace Infrastructure.Interfaces
{
    public interface IDBConnectionFactory : IDisposable
    {
        IDbConnection CreateSqlConnection();
    }
}