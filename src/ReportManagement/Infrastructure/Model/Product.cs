namespace Infrastructure
{
    public class Product
    {
        public int Id { get; set; }
        public long Code { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int StatusProductId { get; set; }
        public int SectionId { get; set; }
        public int ExpirationDate { get; set; }
        public int UnitId { get; set; }
    }
}