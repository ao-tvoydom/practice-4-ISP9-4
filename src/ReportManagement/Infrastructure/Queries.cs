using System;
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
                    new {Id = reportId});

                return response;

            }
        }

        public void InsertReportData(List<ReportData> list)
        {
            //Место соединения всех методов "Insert"
        }

        public void InsertBlockStatus(ReportData reportData)
        {
            BlockStatus blockStatus = new BlockStatus();
            using (var db = new SqlConnection(_connectionString))
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
            using (var db = new SqlConnection(_connectionString))
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
            using (var db = new SqlConnection(_connectionString))
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

        public void InsertDepartmentProduct(ReportData reportData)
        {
            DepartmentProduct departmentProduct = new DepartmentProduct();
            throw new NotImplementedException();
        }

        public void InsertOrder(ReportData reportData)
        {
            Order order = new Order();
            throw new NotImplementedException();
        }

        public void InsertProduct(ReportData reportData)
        {
            Product product = new Product();
            throw new NotImplementedException();
        }

        public void InsertSection(ReportData reportData)
        {
            Section section = new Section();
            throw new NotImplementedException();
        }

        public void InsertStatusProduct(ReportData reportData)
        {
            StatusProduct statusProduct = new StatusProduct();
            using (var db = new SqlConnection(_connectionString))
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
                using (var db = new SqlConnection(_connectionString))
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
