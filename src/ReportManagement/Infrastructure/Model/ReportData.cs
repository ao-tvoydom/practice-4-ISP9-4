using System;

namespace Infrastructure
{
    public class ReportData
    {
        public int Id { get; set; }
        
        public string NameBlockStatus { get; set; }
        
        public string NameBrand { get; set; }
        
        public string NameDepartment { get; set; }
        
        public decimal Realization { get; set; }
        
        public decimal ProductDisposal { get; set; }
        
        public decimal ProductSurplus { get; set; }
        
        public DateOnly LastShipmentDate { get; set; }
        
        public DateOnly LastSaleDate { get; set; }
        
        public decimal SellingPrice { get; set; }
        
        public string NameUnit { get; set; }
        
        public int CodeStatusProduct { get; set; }
        
        public string NameSection { get; set; }
        
        public long CodeProduct { get; set; }
        
        public string NameProduct { get; set; }
        
        public int ExpirationDate { get; set; }
        
    }
}