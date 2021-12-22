using System;

namespace Infrastructure.Model
{
    public class ReportData
    {
        public int ID { get; set; }
        
        public string BrandName { get; set; }
        
        public string DepartmentName { get; set; }
        
        public decimal RealizationQuantity { get; set; }
        
        public decimal RealizationSum { get; set; }
        
        public decimal SurplusQuantity { get; set; }
        
        public long ProductCode { get; set; }
        
        public string ProductName { get; set; }

    }
}