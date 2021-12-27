
namespace Infrastructure.Model
{
    public class Product
    {
        public int ID { get; set; }
        
        public long Code { get; set; }
        
        public string Name { get; set; }
        
        public int BrandID { get; set; }
        
        public int SectionID { get; set; }

        public int ProductStatusID { get; set; }
        
        public int UnitID { get; set; }
        
        public decimal Price { get; set; }
        
        public int ExpirationDate { get; set; }
    }
}