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
    public class DatabaseQueries : IQuery
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public DatabaseQueries(string connectionString)
        {
            _dbConnectionFactory = new DbConnectionFactory(connectionString);
        }

        private SqlConnection GetSqlConnection()
        {
            return (SqlConnection) _dbConnectionFactory.CreateSqlConnection();
        }

        public ReportData GetReportData(int reportId)
        {
            using var db = GetSqlConnection();

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

        public void InsertReportData(List<ReportData> reportDataList)
        {
            using var db = GetSqlConnection();


            db.Open();

            SqlTransaction transaction = db.BeginTransaction();

            try
            {
                foreach (var reportData in reportDataList)
                {
                    ((IQuery)this).InsertBrand(reportData, db);
                    ((IQuery)this).InsertUnit(reportData, db);
                    ((IQuery)this).InsertStatusProduct(reportData, db);
                    ((IQuery)this).InsertSection(reportData, db);
                    ((IQuery)this).InsertProduct(reportData, db);
                    ((IQuery)this).InsertDepartment(reportData, db);
                    var departmentProductId = ((IQuery)this).InsertDepartmentProduct(reportData, db);
                    ((IQuery)this).InsertBlockStatus(reportData, db);
                    ((IQuery)this).InsertOrder(reportData, departmentProductId, db);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        void IQuery.InsertBlockStatus(ReportData reportData, SqlConnection sqlConnection)
        {
            var BlockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus});
            if (BlockStatus.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[BlockStatus](NameBlockStatus) 
                                    VALUES (@NameBlockStatus);",
                    new {NameBlockStatus = reportData.NameBlockStatus});
            }

            var IdBlockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus});
        }

        void IQuery.InsertBrand(ReportData reportData, SqlConnection sqlConnection)
        {
            var Brand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                new {NameBrand = reportData.NameBrand});
            if (Brand.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Brand](NameBrand) 
                                    VALUES (@NameBrand);",
                    new {NameBrand = reportData.NameBrand});
            }

            var IdBrand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                new {NameBrand = reportData.NameBrand});
        }

        void IQuery.InsertDepartment(ReportData reportData, SqlConnection sqlConnection)
        {
            var Department = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                new {NameDepartment = reportData.NameDepartment});
            if (Department.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Department](NameDepartment) 
                                    VALUES (@NameDepartment);",
                    new {NameDepartment = reportData.NameDepartment});
            }

            var IdDepartment = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                new {NameDepartment = reportData.NameDepartment});
        }

        int IQuery.InsertDepartmentProduct(ReportData reportData, SqlConnection sqlConnection)
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
                new {NameDepartment = reportData.NameDepartment});

            departmentProduct.ProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Product
                                    WHERE NameProduct = @NameProduct 
                                    AND CodeProduct = @CodeProduct",
                new
                {
                    NameProduct = reportData.NameProduct,
                    CodeProduct = reportData.CodeProduct
                });
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
                    LastShipmentDate = departmentProduct.LastShipmentDate,
                    LastSaleDate = departmentProduct.LastSaleDate
                });
            int DepartmentProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM DepartmentProduct
                                    ORDER BY Id DESC");

            return DepartmentProductId;
        }

        void IQuery.InsertOrder(ReportData reportData, int departmentProductId, SqlConnection sqlConnection)
        {
            var order = new Order();

            order.SellingPrice = reportData.SellingPrice;
            order.DepartmentProductId = departmentProductId;

            order.BlockStatusId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM BlockStatus
                                    WHERE NameBlockStatus = @NameBlockStatus",
                new {NameBlockStatus = reportData.NameBlockStatus});

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
                });
        }

        void IQuery.InsertProduct(ReportData reportData, SqlConnection sqlConnection)
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
                }));

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
                new {NameBrand = reportData.NameBrand});

            product.StatusProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM StatusProduct
                                    WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct});

            product.SectionId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Section
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection});

            product.UnitId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Unit
                                    WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit});

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
                });
        }

        void IQuery.InsertSection(ReportData reportData, SqlConnection sqlConnection)
        {
            var Section = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection});
            if (Section.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[Section](NameSection) 
                                                VALUES (@NameSection);",
                    new {NameSection = reportData.NameSection});
            }

            var IdSection = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                new {NameSection = reportData.NameSection});
        }

        void IQuery.InsertStatusProduct(ReportData reportData, SqlConnection sqlConnection)
        {
            var StatusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct});
            if (StatusProduct.AsList().Count == 0)
            {
                sqlConnection.Execute(@"INSERT INTO [dbo].[StatusProduct](CodeStatusProduct) 
                                    VALUES (@CodeStatusProduct);",
                    new {CodeStatusProduct = reportData.CodeStatusProduct});
            }

            var IdStatusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                new {CodeStatusProduct = reportData.CodeStatusProduct});
        }

        void IQuery.InsertUnit(ReportData reportData, SqlConnection sqlConnection)
        {
            using var db = GetSqlConnection();

            var Unit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit});
            if (Unit.AsList().Count == 0)
            {
                db.Execute(@"INSERT INTO [dbo].[Unit](NameUnit) 
                                    VALUES (@NameUnit);",
                    new {NameUnit = reportData.NameUnit});
            }

            var IdUnit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
                new {NameUnit = reportData.NameUnit});
        }

        public List<ReportData> GetReportData()
        {
            using var db = GetSqlConnection();

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
