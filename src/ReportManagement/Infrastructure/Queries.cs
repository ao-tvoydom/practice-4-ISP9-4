using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Linq;

namespace Infrastructure
{
    public static class Queries
    {

        private  static readonly string ConnectionString;

        static Queries()
        {
            ConnectionString =
                "Data Source=(local);Initial Catalog=ReportDB;Integrated Security=True;MultipleActiveResultSets=True";
        }
        
        public static List<ReportData> GetReportData()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var response = db.Query<ReportData>(@"SELECT * FROM [dbo].[MainView]");

                return response.ToList();

            }
            
        }

        public static ReportData GetReportData(int reportId)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var response = db.QueryFirst<ReportData>(
                    @"SELECT * FROM [dbo].[MainView] Where Id = @Id",
                    new { Id = reportId});

                return response;

            }
        }
        
        
        
    }
}