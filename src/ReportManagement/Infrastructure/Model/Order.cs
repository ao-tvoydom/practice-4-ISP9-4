namespace Infrastructure.Model
{
    public class Order
    {
        public int Id { get; set; }
        
        public int DepartmentProductId { get; set; }
        
        public int BlockStatusId { get; set; }
        
        public decimal SellingPrice { get; set; }
        
    }
}