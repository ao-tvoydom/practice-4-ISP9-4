using System;
using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Infrastructure.Factory;
using Infrastructure.Model;
using Infrastructure.Interfaces;

namespace Infrastructure
{

    public class ReportRepository : IReportRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;

        public ReportRepository(IDBConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public IReadOnlyCollection<ReportData> GetAll()
        {
            var db = GetSqlConnection();

            var response = db.Query<ReportData>(@"SELECT 
                     BrandName,
                     DepartmentName,
                     RealizationQuantity,
                     RealizationSum,
                     SurplusQuantity,
                     ProductCode,
                     ProductName
                     FROM [dbo].[MainView]
                     ORDER BY ProductCode"
            );

            return response.ToList();
        }

        public ReportData GetById(int reportId)
        {
            var db = GetSqlConnection();
            var response = db.QueryFirst<ReportData>(@"SELECT Id,
                    BrandName,
                    DepartmentName,
                    RealizationQuantity,
                    RealizationSum,
                    SurplusQuantity,
                    ProductCode,
                    ProductName 
                    FROM [dbo].[MainView] 
                    Where Id = @Id",
                new {Id = reportId});

            return response;
        }

        public void Add(IReadOnlyCollection<ReportData> reportDataList)
        {
            var db = GetSqlConnection();
            using var transaction = db.BeginTransaction();

            try
            {
                foreach (var reportData in reportDataList)
                {
                    InsertBrand(reportData, db, transaction);
                    InsertProduct(reportData, db, transaction);
                    InsertDepartment(reportData, db, transaction);
                    InsertSale(reportData, db, transaction);
                }
                transaction.Commit();
            }
            catch (Exception ex1)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    throw;
                }

                throw;
            }
            Dispose();
        }

        private SqlConnection GetSqlConnection()
        {
            return (SqlConnection) _dbConnectionFactory.CreateSqlConnection();
        }
        
        private void InsertBrand(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var brand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE BrandName = @BrandName",
                new {BrandName = reportData.BrandName}, transaction);
            if (brand.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Brand](BrandName) 
                                    VALUES (@BrandName);",
                    new {BrandName = reportData.BrandName}, transaction);
            }

            var idBrand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE BrandName = @BrandName",
                new {BrandName = reportData.BrandName}, transaction);
        }

        private void InsertDepartment(ReportData reportData, SqlConnection sqlConnection,  SqlTransaction transaction)
        {
            var department = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE DepartmentName = @DepartmentName",
                new {DepartmentName = reportData.DepartmentName}, transaction);
            if (department.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Department](DepartmentName) 
                                    VALUES (@DepartmentName);",
                    new {DepartmentName = reportData.DepartmentName}, transaction);
            }

            var idDepartment = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE DepartmentName = @DepartmentName",
                new {DepartmentName = reportData.DepartmentName}, transaction);
        }

        //TODO: Find product by code + name
        private void InsertSale(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var sale = new Sale();

            sale.RealizationQuantity = reportData.RealizationQuantity;
            sale.RealizationSum = reportData.RealizationSum;
            sale.SurplusQuantity = reportData.SurplusQuantity;

            sale.DepartmentID = sqlConnection.QueryFirst<int>(@"SELECT DepartmentID
                                    FROM Department
                                    WHERE DepartmentName = @DepartmentName",
                new {DepartmentName = reportData.DepartmentName}, transaction);
            
            sale.ProductID = sqlConnection.QueryFirst<int>(@"SELECT ProductID
                                    FROM Product
                                    WHERE ProductName = @ProductName",
                new {ProductName = reportData.ProductName}, transaction);

            sqlConnection.Execute(@"INSERT INTO [dbo].[Sale]([DepartmentID],
                                                            [ProductID],
                                                            [RealizationQuantity],
                                                            [RealizationSum],
                                                            [SurplusQuantity])
                                                    VALUES (@DepartmentID,
                                                            @ProductID,
                                                            @RealizationQuantity,
                                                            @RealizationSum,
                                                            @SurplusQuantity);",
                new
                {
                    DepartmentID = sale.DepartmentID,
                    ProductID = sale.ProductID,
                    RealizationQuantity = sale.RealizationQuantity,
                    RealizationSum = sale.RealizationSum,
                    SurplusQuantity = sale.SurplusQuantity,
                }, transaction);
        }

        private void InsertProduct(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var product = new Product();

            var duplicateProducts = new List<Product>();


            duplicateProducts.AddRange(sqlConnection.Query<Product>(@"SELECT *
                                    FROM Product
                                    WHERE ProductCode = @ProductCode
                                    AND ProductName = @ProductName",
                new
                {
                    ProductCode = reportData.ProductCode,
                    ProductName = reportData.ProductName
                }, transaction));

            if (duplicateProducts.Count != 0)
            {
                return;
            }

            product.Code = reportData.ProductCode;
            product.Name = reportData.ProductName;

            product.BrandID = sqlConnection.QueryFirst<int>(@"SELECT BrandID
                                    FROM Brand
                                    WHERE BrandName = @BrandName",
                new {BrandName = reportData.BrandName}, transaction);

            sqlConnection.Execute(@"INSERT INTO [dbo].[Product]([ProductCode],
                                                            [ProductName],
                                                            [BrandID])
                                                    VALUES (@ProductCode,
                                                            @ProductName,
                                                            @BrandID);",
                new
                {
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    BrandID = product.BrandID,
                }, transaction);
        }
        
        public IReadOnlyCollection<PivotData> GetPivotData()
        {
            var db = GetSqlConnection();

            var response = db.Query<PivotData>(@"SELECT 
                     ProductCode,
                     ProductName,
                     BrandName,
                     DepartmentName,
                     RealizationQuantityTotal,
                     RealizationSumTotal,
                     SurplusQuantityTotal
                     FROM [dbo].[PivotView]
                     "
            );

            return response.ToList();
        }

        public void Dispose()
        {
            _dbConnectionFactory?.Dispose();
        }
    }
}