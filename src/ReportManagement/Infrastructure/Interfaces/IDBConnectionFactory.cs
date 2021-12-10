using System.Data;

namespace Infrastructure.Interfaces
{
    public interface IDBConnectionFactory
    {
        IDbConnection CreateSqlServerConnection();
    }
}