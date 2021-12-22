namespace Infrastructure.Model
{
    public class Sale
    {
        public int Id { get; set; }
        
        public int DepartmentId { get; set; }

        public int ProductId { get; set; }
        
        public decimal RealizationQuantity { get; set; }

        public decimal RealizationSum { get; set; }

        public decimal SurplusQuantity { get; set; }
        
    }
}