using System;

namespace Infrastructure.Model
{
    public class ReportData
    {
        public int Id { get; set; }
        
        public string BlockStatusName { get; set; }
        
        public string BrandName { get; set; }
        
        public string DepartmentName { get; set; }
        
        public decimal RealizationQuantity { get; set; }
        
        public decimal Disposal { get; set; }
        
        public decimal SurplusQuantity { get; set; }
        
        public DateOnly LastShipmentDate { get; set; }
        
        public DateOnly LastSaleDate { get; set; }
        
        public decimal Price { get; set; }
        
        public string UnitName { get; set; }
        
        public int ProductStatusCode { get; set; }
        
        public string SectionName { get; set; }
        
        public long ProductCode { get; set; }
        
        public string ProductName { get; set; }
        
        public int ExpirationDate { get; set; }

    }
}