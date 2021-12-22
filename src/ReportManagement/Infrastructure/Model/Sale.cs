namespace Infrastructure.Model
{
    public class Sale
    {
        public int ID { get; set; }
        
        public int DepartmentID { get; set; }

        public int ProductID { get; set; }
        
        public decimal RealizationQuantity { get; set; }

        public decimal RealizationSum { get; set; }

        public decimal SurplusQuantity { get; set; }
        
    }
}