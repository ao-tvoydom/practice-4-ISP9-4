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
            return (SqlConnection)_dbConnectionFactory.CreateSqlServerConnection();
        }

        ReportData IQuery.GetReportData(int reportId)
        {
            using (var db = GetSqlConnection())
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
                    new {Id = reportId});

                return response;

            }
        }

        public void InsertReportData(List<ReportData> reportDataList)
        {
            using (var db =  GetSqlConnection())
            {
                foreach (var reportData in reportDataList)
                {
                    InsertBrand(reportData);
                    InsertUnit(reportData);
                    InsertStatusProduct(reportData);
                    InsertSection(reportData);
                    InsertProduct(reportData);
                    InsertDepartment(reportData);
                    int departmentProductId = InsertDepartmentProduct(reportData);
                    InsertBlockStatus(reportData);
                    InsertOrder(reportData, departmentProductId);
                }
            }
        }

        public void InsertBlockStatus(ReportData reportData)
        {
            BlockStatus blockStatus = new BlockStatus();
            using (var db =  GetSqlConnection())
            {
                var BlockStatus = db.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                    new {NameBlockStatus = reportData.NameBlockStatus});
                if (BlockStatus.AsList().Count == 0)
                {
                    db.Execute(@"INSERT INTO [dbo].[BlockStatus](NameBlockStatus) 
                                    VALUES (@NameBlockStatus);",
                        new {NameBlockStatus = reportData.NameBlockStatus});
                }

                var IdBlockStatus = db.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
                    new {NameBlockStatus = reportData.NameBlockStatus});
            }
        }

        public void InsertBrand(ReportData reportData)
        {
            Brand brand = new Brand();
            using (var db =  GetSqlConnection())
            {
                var Brand = db.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                    new {NameBrand = reportData.NameBrand});
                if (Brand.AsList().Count == 0)
                {
                    db.Execute(@"INSERT INTO [dbo].[Brand](NameBrand) 
                                    VALUES (@NameBrand);",
                        new {NameBrand = reportData.NameBrand});
                }

                var IdBrand = db.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
                    new {NameBrand = reportData.NameBrand});
            }
        }

        public void InsertDepartment(ReportData reportData)
        {
            Department department = new Department();
            using (var db = GetSqlConnection())
            {
                var Department = db.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                    new {NameDepartment = reportData.NameDepartment});
                if (Department.AsList().Count == 0)
                {
                    db.Execute(@"INSERT INTO [dbo].[Department](NameDepartment) 
                                    VALUES (@NameDepartment);",
                        new {NameDepartment = reportData.NameDepartment});
                }

                var IdDepartment = db.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
                    new {NameDepartment = reportData.NameDepartment});
            }
        }

        public int InsertDepartmentProduct(ReportData reportData)
        {
            DepartmentProduct departmentProduct = new DepartmentProduct();
            departmentProduct.Realization = reportData.Realization;
            departmentProduct.ProductDisposal = reportData.ProductDisposal;
            departmentProduct.ProductSurplus = reportData.ProductSurplus;
            departmentProduct.LastSaleDate = reportData.LastSaleDate;
            departmentProduct.LastShipmentDate = reportData.LastShipmentDate;
            using (var db = GetSqlConnection())
            {
                departmentProduct.DepartmentId = db.QueryFirst<int>(@"SELECT Id
                                    FROM Department
                                    WHERE NameDepartment = @NameDepartment",
                    new{NameDepartment = reportData.NameDepartment});
                
                departmentProduct.ProductId = db.QueryFirst<int>(@"SELECT Id
                                    FROM Product
                                    WHERE NameProduct = @NameProduct 
                                    AND CodeProduct = @CodeProduct",
                    new
                    {
                        NameProduct = reportData.NameProduct,
                        CodeProduct = reportData.CodeProduct
                    });
                db.Execute(@"INSERT INTO [dbo].[DepartmentProduct]([DepartmentId],
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
                int DepartmentProductId = db.QueryFirst<int>(@"SELECT Id
                                    FROM DepartmentProduct
                                    ORDER BY Id DESC");

                return DepartmentProductId;
            }
        }

        public void InsertOrder(ReportData reportData, int departmentProductId)
        {
            Order order = new Order();
            order.SellingPrice = reportData.SellingPrice;
            order.DepartmentProductId = departmentProductId;
            using (var db = GetSqlConnection())
            {
                order.BlockStatusId = db.QueryFirst<int>(@"SELECT Id
                                    FROM BlockStatus
                                    WHERE NameBlockStatus = @NameBlockStatus",
                    new{NameBlockStatus = reportData.NameBlockStatus});
                
                db.Execute(@"INSERT INTO [dbo].[Order]([DepartmentProductId],
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
        }

        public void InsertProduct(ReportData reportData)
        {
            Product product = new Product();
            List<Product> DublicateProducts = new List<Product>();
            using (var db = GetSqlConnection())
            {
                DublicateProducts.AddRange(db.Query<Product>(@"SELECT *
                                    FROM Product
                                    WHERE CodeProduct = @CodeProduct
                                    AND NameProduct = @NameProduct",
                    new
                    {
                        CodeProduct = reportData.CodeProduct,
                        NameProduct = reportData.NameProduct
                    }));
            }

            if (DublicateProducts.Count != 0)
            {
                return;
            }
            product.Code = reportData.CodeProduct;
            product.Name = reportData.NameProduct;
            product.ExpirationDate = reportData.ExpirationDate;
            using (var db = GetSqlConnection())
            {
                product.BrandId = db.QueryFirst<int>(@"SELECT Id
                                    FROM Brand
                                    WHERE NameBrand = @NameBrand",
                                    new{NameBrand = reportData.NameBrand});
                
                product.StatusProductId = db.QueryFirst<int>(@"SELECT Id
                                    FROM StatusProduct
                                    WHERE CodeStatusProduct = @CodeStatusProduct",
                    new{CodeStatusProduct = reportData.CodeStatusProduct});
                
                product.SectionId = db.QueryFirst<int>(@"SELECT Id
                                    FROM Section
                                    WHERE NameSection = @NameSection",
                    new{NameSection = reportData.NameSection});
                
                product.UnitId = db.QueryFirst<int>(@"SELECT Id
                                    FROM Unit
                                    WHERE NameUnit = @NameUnit",
                    new{NameUnit = reportData.NameUnit});
                
                db.Execute(@"INSERT INTO [dbo].[Product]([CodeProduct],
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
        }

        public void InsertSection(ReportData reportData)
        {
            Section section = new Section();
            using (var db = GetSqlConnection())
                        {
                            var Section = db.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                                new {NameSection = reportData.NameSection});
                            if (Section.AsList().Count == 0)
                            {
                                db.Execute(@"INSERT INTO [dbo].[Section](NameSection) 
                                                VALUES (@NameSection);",
                                    new {NameSection = reportData.NameSection});
                            }
            
                            var IdSection = db.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
                                new {NameSection = reportData.NameSection});
                        }
        }

        public void InsertStatusProduct(ReportData reportData)
        {
            StatusProduct statusProduct = new StatusProduct();
            using (var db = GetSqlConnection())
            {
                var StatusProduct = db.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                    new {CodeStatusProduct = reportData.CodeStatusProduct});
                if (StatusProduct.AsList().Count == 0)
                {
                    db.Execute(@"INSERT INTO [dbo].[StatusProduct](CodeStatusProduct) 
                                    VALUES (@CodeStatusProduct);",
                        new {CodeStatusProduct = reportData.CodeStatusProduct});
                }

                var IdStatusProduct = db.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
                    new {CodeStatusProduct = reportData.CodeStatusProduct});
            }
        }

        public void InsertUnit(ReportData reportData)
            {
                Unit unit = new Unit();
                using (var db = GetSqlConnection())
                {
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
            }
        List < ReportData > IQuery.GetReportData()
            {
                using (var db = GetSqlConnection())
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
