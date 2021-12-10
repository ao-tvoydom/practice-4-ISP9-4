using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Infrastructure.Model;
using Infrastructure.Interfaces;

namespace Infrastructure
{
    public class DatabaseQueries : IQuery
    {

        private readonly string _connectionString;

        public DatabaseQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        ReportData IQuery.GetReportData(int reportId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var response = db.QueryFirst<ReportData>(@"SELECT Id,
                    NameBlockStatus,
                    NameBrand,
                    NameDepartment,
                    Realization,
                    ProductDisposal,
                    ProductSurplus,
                    LastShipmentDate,
                    LastSaleDate,
                    SellingPrice,
                    NameUnit,
                    CodeStatusProduct,
                    NameSection,
                    CodeProduct,
                    NameProduct,
                    ExpirationDate 
                    FROM [dbo].[MainView] 
                    Where Id = @Id",
                    new { Id = reportId});

                return response;

            }
        } 

        List<ReportData> IQuery.GetReportData()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var response = db.Query<ReportData>(@"SELECT Id,
                     NameBlockStatus,
                     NameBrand,
                     NameDepartment,
                     Realization,
                     ProductDisposal,
                     ProductSurplus,
                     LastShipmentDate,
                     LastSaleDate,
                     SellingPrice,
                     NameUnit,
                     CodeStatusProduct,
                     NameSection,
                     CodeProduct,
                     NameProduct,
                     ExpirationDate
                     FROM [dbo].[MainView]");

                return response.ToList();

            }
        }
        
    }
}