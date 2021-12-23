namespace Infrastructure.Model
{
    public class PivotData
    {
        public long ProductCode { get; set; }
        
        public string ProductName { get; set; }
        
        public string BrandName { get; set; }

        public string DepartmentName { get; set; }

        public decimal RealizationQuantityTotal { get; set; }

        public decimal RealizationSumTotal { get; set; }

        public decimal SurplusQuantityTotal { get; set; }

        
    }
}