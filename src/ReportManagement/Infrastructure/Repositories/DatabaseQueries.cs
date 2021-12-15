using System;
using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using Infrastructure.Factory;
using Infrastructure.Model;
using Infrastructure.Interfaces;

namespace Infrastructure;

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
            new { Id = reportId });

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
                InsertBrand(reportData, db);
                InsertUnit(reportData, db);
                InsertStatusProduct(reportData, db);
                InsertSection(reportData, db);
                InsertProduct(reportData, db);
                InsertDepartment(reportData, db);
                var departmentProductId = InsertDepartmentProduct(reportData, db);
                InsertBlockStatus(reportData, db);
                InsertOrder(reportData, departmentProductId, db);
            }

            transaction.Commit();
        }
        catch (Exception ex1)
        {
            Console.WriteLine($"Exception message: {ex1.Message}");

            try
            {
                transaction.Rollback();
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Exception message: {ex2.Message}");
                throw;
            }
                
            throw;
        }
    }

    private SqlConnection GetSqlConnection()
    {
        return (SqlConnection)_dbConnectionFactory.CreateSqlConnection();
    }

    private void InsertBlockStatus(ReportData reportData, SqlConnection sqlConnection)
    {
        var blockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
            new { NameBlockStatus = reportData.NameBlockStatus });
        if (blockStatus.AsList().Count == 0)
        {
            sqlConnection.Execute(@"INSERT INTO [dbo].[BlockStatus](NameBlockStatus) 
                                    VALUES (@NameBlockStatus);",
                new { NameBlockStatus = reportData.NameBlockStatus });
        }

        var idBlockStatus = sqlConnection.Query<BlockStatus>(@"SELECT * 
                        FROM BlockStatus 
                        WHERE NameBlockStatus = @NameBlockStatus",
            new { NameBlockStatus = reportData.NameBlockStatus });
    }

    private void InsertBrand(ReportData reportData, SqlConnection sqlConnection)
    {
        var brand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
            new { NameBrand = reportData.NameBrand });
        if (brand.AsList().Count == 0)
        {
            sqlConnection.Execute(@"INSERT INTO [dbo].[Brand](NameBrand) 
                                    VALUES (@NameBrand);",
                new { NameBrand = reportData.NameBrand });
        }

        var idBrand = sqlConnection.Query<Brand>(@"SELECT * 
                        FROM Brand 
                        WHERE NameBrand = @NameBrand",
            new { NameBrand = reportData.NameBrand });
    }

    private void InsertDepartment(ReportData reportData, SqlConnection sqlConnection)
    {
        var department = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
            new { NameDepartment = reportData.NameDepartment });
        if (department.AsList().Count == 0)
        {
            sqlConnection.Execute(@"INSERT INTO [dbo].[Department](NameDepartment) 
                                    VALUES (@NameDepartment);",
                new { NameDepartment = reportData.NameDepartment });
        }

        var idDepartment = sqlConnection.Query<Department>(@"SELECT * 
                        FROM Department 
                        WHERE NameDepartment = @NameDepartment",
            new { NameDepartment = reportData.NameDepartment });
    }

    private int InsertDepartmentProduct(ReportData reportData, SqlConnection sqlConnection)
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
            new { NameDepartment = reportData.NameDepartment });

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
        int departmentProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM DepartmentProduct
                                    ORDER BY Id DESC");

        return departmentProductId;
    }

    private void InsertOrder(ReportData reportData, int departmentProductId, SqlConnection sqlConnection)
    {
        var order = new Order();

        order.SellingPrice = reportData.SellingPrice;
        order.DepartmentProductId = departmentProductId;

        order.BlockStatusId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM BlockStatus
                                    WHERE NameBlockStatus = @NameBlockStatus",
            new { NameBlockStatus = reportData.NameBlockStatus });

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

    private void InsertProduct(ReportData reportData, SqlConnection sqlConnection)
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
            new { NameBrand = reportData.NameBrand });

        product.StatusProductId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM StatusProduct
                                    WHERE CodeStatusProduct = @CodeStatusProduct",
            new { CodeStatusProduct = reportData.CodeStatusProduct });

        product.SectionId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Section
                                    WHERE NameSection = @NameSection",
            new { NameSection = reportData.NameSection });

        product.UnitId = sqlConnection.QueryFirst<int>(@"SELECT Id
                                    FROM Unit
                                    WHERE NameUnit = @NameUnit",
            new { NameUnit = reportData.NameUnit });

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

    private void InsertSection(ReportData reportData, SqlConnection sqlConnection)
    {
        var section = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
            new { NameSection = reportData.NameSection });
        if (section.AsList().Count == 0)
        {
            sqlConnection.Execute(@"INSERT INTO [dbo].[Section](NameSection) 
                                                VALUES (@NameSection);",
                new { NameSection = reportData.NameSection });
        }

        var idSection = sqlConnection.Query<Section>(@"SELECT * 
                                    FROM Section 
                                    WHERE NameSection = @NameSection",
            new { NameSection = reportData.NameSection });
    }

    private void InsertStatusProduct(ReportData reportData, SqlConnection sqlConnection)
    {
        var statusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
            new { CodeStatusProduct = reportData.CodeStatusProduct });
        if (statusProduct.AsList().Count == 0)
        {
            sqlConnection.Execute(@"INSERT INTO [dbo].[StatusProduct](CodeStatusProduct) 
                                    VALUES (@CodeStatusProduct);",
                new { CodeStatusProduct = reportData.CodeStatusProduct });
        }

        var idStatusProduct = sqlConnection.Query<StatusProduct>(@"SELECT * 
                        FROM StatusProduct 
                        WHERE CodeStatusProduct = @CodeStatusProduct",
            new { CodeStatusProduct = reportData.CodeStatusProduct });
    }

    private void InsertUnit(ReportData reportData, SqlConnection sqlConnection)
    {
        var db = GetSqlConnection();
            
        var unit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
            new { NameUnit = reportData.NameUnit });
        if (unit.AsList().Count == 0)
        {
            db.Execute(@"INSERT INTO [dbo].[Unit](NameUnit) 
                                    VALUES (@NameUnit);",
                new { NameUnit = reportData.NameUnit });
        }

        var idUnit = db.Query<Unit>(@"SELECT * 
                        FROM Unit 
                        WHERE NameUnit = @NameUnit",
            new { NameUnit = reportData.NameUnit });
    }

    public void Dispose()
    {
        _dbConnectionFactory?.Dispose();
    }
}