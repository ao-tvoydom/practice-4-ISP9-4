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
                     Disposal,
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
                    Disposal,
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
                    var a = reportData;
                    InsertSection(reportData, db, transaction);
                    InsertUnit(reportData, db, transaction);
                    InsertBlockStatus(reportData, db, transaction);
                    InsertBrand(reportData, db, transaction);
                    InsertDepartment(reportData, db, transaction);
                    InsertProductStatus(reportData, db, transaction);
                    InsertProduct(reportData, db, transaction);
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
        
        private void InsertBlockStatus(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var blockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE BlockStatusName = @BlockStatusName",
                new {BlockStatusName = reportData.BlockStatusName}, transaction);
            if (blockStatus.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[BlockStatus](BlockStatusName) 
                                    VALUES (@BlockStatusName);",
                    new {BlockStatusName = reportData.BlockStatusName}, transaction);
            }

            var idBlockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE BlockStatusName = @BlockStatusName",
                new {BlockStatusName = reportData.BlockStatusName}, transaction);
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

        private void InsertSale(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var sale = new Sale();

            sale.RealizationQuantity = reportData.RealizationQuantity;
            sale.SurplusQuantity = reportData.SurplusQuantity;
            sale.Disposal = reportData.Disposal;
            sale.LastShipmentDate = reportData.LastShipmentDate;
            sale.LastSaleDate = reportData.LastSaleDate;

            sale.DepartmentID = sqlConnection.QueryFirst<int>(@"SELECT DepartmentID
                                    FROM Department
                                    WHERE DepartmentName = @DepartmentName",
                new {DepartmentName = reportData.DepartmentName}, transaction);
            
            sale.ProductID = sqlConnection.QueryFirst<int>(@"SELECT ProductID
                                    FROM Product
                                    WHERE ProductName = @ProductName 
                                    AND ProductCode = @ProductCode",
                new
                {
                    ProductName = reportData.ProductName,
                    ProductCode = reportData.ProductCode
                }, transaction);
            sale.BlockStatusID = sqlConnection.QueryFirst<int>(@"SELECT BlockStatusID
                                    FROM BlockStatus
                                    WHERE BlockStatusName = @BlockStatusName",
                new {BlockStatusName = reportData.BlockStatusName}, transaction);

            sqlConnection.Execute(@"INSERT INTO [dbo].[Sale]([DepartmentID],
                                                            [ProductID],
                                                            [RealizationQuantity],
                                                            [SurplusQuantity],
                                                            [BlockStatusID],
                                                            [LastShipmentDate],
                                                            [LastSaleDate],
                                                            [Disposal])
                                                    VALUES (@DepartmentID,
                                                            @ProductID,
                                                            @RealizationQuantity,
                                                            @SurplusQuantity,
                                                            @BlockStatusID,
                                                            @LastShipmentDate,
                                                            @LastSaleDate,
                                                            @Disposal);",
                new
                {
                    DepartmentID = sale.DepartmentID,
                    ProductID = sale.ProductID,
                    RealizationQuantity = sale.RealizationQuantity,
                    SurplusQuantity = sale.SurplusQuantity,
                    BlockStatusID = sale.BlockStatusID,
                    LastShipmentDate = sale.LastShipmentDate,
                    LastSaleDate = sale.LastSaleDate,
                    Disposal = sale.Disposal
                }, transaction);
        }

        private void InsertProduct(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var product = new Product();

            var duplicateProducts = new List<Product>();


            duplicateProducts.AddRange(sqlConnection.Query<Product>(@"SELECT *
                                    FROM Product
                                    WHERE ProductCode = @ProductCode",
                new
                {
                    ProductCode = reportData.ProductCode
                }, transaction));

            if (duplicateProducts.Count != 0)
            {
                return;
            }

            product.Code = reportData.ProductCode;
            product.Name = reportData.ProductName;
            product.Price = reportData.Price;
            product.ExpirationDate = reportData.ExpirationDate;

            product.BrandID = sqlConnection.QueryFirst<int>(@"SELECT BrandID
                                    FROM Brand
                                    WHERE BrandName = @BrandName",
                new {BrandName = reportData.BrandName}, transaction);
            
            product.SectionID = sqlConnection.QueryFirst<int>(@"SELECT SectionID
                                    FROM Section 
                                    WHERE SectionName = @SectionName",
                new {SectionName = reportData.SectionName}, transaction);
            
            product.ProductStatusID = sqlConnection.QueryFirst<int>(@"SELECT ProductStatusID
                                    FROM ProductStatus 
                                    WHERE ProductStatusCode = @ProductStatusCode",
                new {ProductStatusCode = reportData.ProductStatusCode}, transaction);
            
            product.UnitID = sqlConnection.QueryFirst<int>(@"SELECT UnitID
                                    FROM Unit 
                                    WHERE UnitName = @UnitName",
                new {UnitName = reportData.UnitName}, transaction);
            
            sqlConnection.Execute(@"INSERT INTO [dbo].[Product]([ProductCode],
                                                            [ProductName],
                                                            [BrandID],
                                                            [SectionID],
                                                            [ProductStatusID],
                                                            [UnitID],
                                                            [Price],
                                                            [ExpirationDate])
                                                    VALUES (@ProductCode,
                                                            @ProductName,
                                                            @BrandID,
                                                            @SectionID,
                                                            @ProductStatusID,
                                                            @UnitID,
                                                            @Price,
                                                            @ExpirationDate);",
                new
                {
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    BrandID = product.BrandID,
                    SectionID = product.SectionID,
                    ProductStatusID = product.ProductStatusID,
                    UnitID = product.UnitID,
                    Price = product.Price,
                    ExpirationDate = product.ExpirationDate
                }, transaction);
        }

        private void InsertSection(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var section = sqlConnection.Query<Section>(@"SELECT * 
                        FROM Section 
                        WHERE SectionName = @SectionName",
                new {SectionName = reportData.SectionName}, transaction);
            if (section.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Section](SectionName) 
                                    VALUES (@SectionName);",
                    new {SectionName = reportData.SectionName}, transaction);
            }

            var idSection = sqlConnection.Query<Section>(@"SELECT * 
                        FROM Section 
                        WHERE SectionName = @SectionName",
                new {SectionName = reportData.SectionName}, transaction);
        }
        
        private void InsertProductStatus(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var productStatus = sqlConnection.Query<ProductStatus>(@"SELECT * 
                        FROM ProductStatus 
                        WHERE ProductStatusCode = @ProductStatusCode",
                new {ProductStatusCode = reportData.ProductStatusCode}, transaction);
            if (productStatus.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[ProductStatus](ProductStatusCode) 
                                    VALUES (@ProductStatusCode);",
                    new {ProductStatusCode = reportData.ProductStatusCode}, transaction);
            }

            var idProductStatus = sqlConnection.Query<ProductStatus>(@"SELECT * 
                        FROM ProductStatus 
                        WHERE ProductStatusCode = @ProductStatusCode",
                new {ProductStatusCode = reportData.ProductStatusCode}, transaction);
        }
        
        private void InsertUnit(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var unit = sqlConnection.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE UnitName = @UnitName",
                new {UnitName = reportData.UnitName}, transaction);
            if (unit.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Unit](UnitName) 
                                    VALUES (@UnitName);",
                    new {UnitName = reportData.UnitName}, transaction);
            }

            var idUnit = sqlConnection.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE UnitName = @UnitName",
                new {UnitName = reportData.UnitName}, transaction);
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
                     Disposal,
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