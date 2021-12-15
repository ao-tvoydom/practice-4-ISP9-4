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
        private readonly DbConnectionFactory _dbConnectionFactory;

        public ReportRepository(string connectionString)
        {
            _dbConnectionFactory = new DbConnectionFactory(connectionString);
        }

        public IReadOnlyCollection<ReportData> GetAll()
        {
            var db = GetSqlConnection();

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

        public ReportData GetById(int reportId)
        {
            var db = GetSqlConnection();
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
                    InsertUnit(reportData, db, transaction);
                    InsertStatusProduct(reportData, db, transaction);
                    InsertSection(reportData, db, transaction);
                    InsertProduct(reportData, db, transaction);
                    InsertDepartment(reportData, db, transaction);
                    var departmentProductId = InsertDepartmentProduct(reportData, db, transaction);
                    InsertBlockStatus(reportData, db, transaction);
                    InsertOrder(reportData, departmentProductId, db, transaction);
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
        }

        private SqlConnection GetSqlConnection()
        {
            return (SqlConnection) _dbConnectionFactory.CreateSqlConnection();
        }

        private void InsertBlockStatus(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var blockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus}, transaction);
            if (blockStatus.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[BlockStatus](NameBlockStatus) 
                                    VALUES (@NameBlockStatus);",
                    new {NameBlockStatus = reportData.NameBlockStatus}, transaction);
            }

            var idBlockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus}, transaction);
        }

        private void InsertBrand(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var brand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                new {NameBrand = reportData.NameBrand}, transaction);
            if (brand.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Brand](NameBrand) 
                                    VALUES (@NameBrand);",
                    new {NameBrand = reportData.NameBrand}, transaction);
            }

            var idBrand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                new {NameBrand = reportData.NameBrand}, transaction);
        }

        private void InsertDepartment(ReportData reportData, SqlConnection sqlConnection,  SqlTransaction transaction)
        {
            var department = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                new {NameDepartment = reportData.NameDepartment}, transaction);
            if (department.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Department](NameDepartment) 
                                    VALUES (@NameDepartment);",
                    new {NameDepartment = reportData.NameDepartment}, transaction);
            }

            var idDepartment = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                new {NameDepartment = reportData.NameDepartment}, transaction);
        }

        private int InsertDepartmentProduct(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var departmentProduct = new DepartmentProduct();

            departmentProduct.Realization = reportData.Realization;
            departmentProduct.ProductDisposal = reportData.ProductDisposal;
            departmentProduct.ProductSurplus = reportData.ProductSurplus;
            departmentProduct.LastSaleDate = reportData.LastSaleDate;
            departmentProduct.LastShipmentDate = reportData.LastShipmentDate;

            departmentProduct.DepartmentId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Department
                                    WHERE NameDepartment = @NameDepartment",
                new {NameDepartment = reportData.NameDepartment}, transaction);

            departmentProduct.ProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Product
                                    WHERE NameProduct = @NameProduct 
                                    AND CodeProduct = @CodeProduct",
                new
                {
                    NameProduct = reportData.NameProduct,
                    CodeProduct = reportData.CodeProduct
                }, transaction);
            sqlConnection.Execute(@"INSERT INTO [dbo].[DepartmentProduct]([DepartmentId],
                                                            [ProductId],
                                                            [Realization],
                                                            [ProductDisposal],
                                                            [ProductSurplus],
                                                            [LastShipmentDate],
                                                            [LastSaleDate]) 
                                                     VALUES (@DepartmentId,
                                                            @ProductId,
                                                            @Realization,
                                                            @ProductDisposal,
                                                            @ProductSurplus,
                                                            @LastShipmentDate,
                                                            @LastSaleDate);",
                new
                {
                    DepartmentId = departmentProduct.DepartmentId,
                    ProductId = departmentProduct.ProductId,
                    Realization = departmentProduct.Realization,
                    ProductDisposal = departmentProduct.ProductDisposal,
                    ProductSurplus = departmentProduct.ProductSurplus,
                    LastShipmentDate = departmentProduct.LastShipmentDate.ToDateTime(TimeOnly.Parse("00:00 AM")),
                    LastSaleDate = departmentProduct.LastSaleDate.ToDateTime(TimeOnly.Parse("00:00 AM"))
                }, transaction);
            int departmentProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM DepartmentProduct
                                    ORDER BY Id DESC", transaction: transaction);

            return departmentProductId;
        }

        private void InsertOrder(ReportData reportData, int departmentProductId, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var order = new Order();

            order.SellingPrice = reportData.SellingPrice;
            order.DepartmentProductId = departmentProductId;

            order.BlockStatusId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM BlockStatus
                                    WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus}, transaction);

            sqlConnection.Execute(@"INSERT INTO [dbo].[Order]([DepartmentProductId],
                                                            [BlockStatusId],
                                                            [SellingPrice])
                                                    VALUES (@DepartmentProductId,
                                                            @BlockStatusId,
                                                            @SellingPrice);",
                new
                {
                    DepartmentProductId = order.DepartmentProductId,
                    BlockStatusId = order.BlockStatusId,
                    SellingPrice = order.SellingPrice,
                }, transaction);
        }

        private void InsertProduct(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var product = new Product();

            List<Product> duplicateProducts = new List<Product>();


            duplicateProducts.AddRange(sqlConnection.Query<Product>(@"SELECT *
                                    FROM Product
                                    WHERE CodeProduct = @CodeProduct
                                    AND NameProduct = @NameProduct",
                new
                {
                    CodeProduct = reportData.CodeProduct,
                    NameProduct = reportData.NameProduct
                }, transaction));

            if (duplicateProducts.Count != 0)
            {
                return;
            }

            product.Code = reportData.CodeProduct;
            product.Name = reportData.NameProduct;
            product.ExpirationDate = reportData.ExpirationDate;


            product.BrandId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Brand
                                    WHERE NameBrand = @NameBrand",
                new {NameBrand = reportData.NameBrand}, transaction);

            product.StatusProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM StatusProduct
                                    WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct}, transaction);

            product.SectionId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Section
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection}, transaction);

            product.UnitId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Unit
                                    WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit}, transaction);

            sqlConnection.Execute(@"INSERT INTO [dbo].[Product]([CodeProduct],
                                                            [NameProduct],
                                                            [BrandId],
                                                            [StatusProductId],
                                                            [SectionId],
                                                            [ExpirationDate],
                                                            [UnitId])
                                                    VALUES (@CodeProduct,
                                                            @NameProduct,
                                                            @BrandId,
                                                            @StatusProductId,
                                                            @SectionId,
                                                            @ExpirationDate,
                                                            @UnitId);",
                new
                {
                    CodeProduct = product.Code,
                    NameProduct = product.Name,
                    BrandId = product.BrandId,
                    StatusProductId = product.StatusProductId,
                    SectionId = product.SectionId,
                    ExpirationDate = product.ExpirationDate,
                    UnitId = product.UnitId
                }, transaction);
        }

        private void InsertSection(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var section = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection}, transaction);
            if (section.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Section](NameSection) 
                                                VALUES (@NameSection);",
                    new {NameSection = reportData.NameSection}, transaction);
            }

            var idSection = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection}, transaction);
        }

        private void InsertStatusProduct(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var statusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct}, transaction);
            if (statusProduct.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[StatusProduct](CodeStatusProduct) 
                                    VALUES (@CodeStatusProduct);",
                    new {CodeStatusProduct = reportData.CodeStatusProduct}, transaction);
            }

            var idStatusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct}, transaction);
        }

        private void InsertUnit(ReportData reportData, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            var db = GetSqlConnection();

            var unit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit}, transaction);
            if (unit.AsList().Count == 0)
            {
                db.Execute(@"INSERT INTO [dbo].[Unit](NameUnit) 
                                    VALUES (@NameUnit);",
                    new {NameUnit = reportData.NameUnit}, transaction);
            }

            var idUnit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit}, transaction);
        }

        public void Dispose()
        {
            _dbConnectionFactory?.Dispose();
        }
    }
}