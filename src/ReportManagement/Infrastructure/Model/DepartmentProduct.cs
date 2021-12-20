using  System;

namespace Infrastructure.Model
{
    public class DepartmentProduct
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        
        public int ProductId { get; set; }
        
        public decimal Realization { get; set; }
        
        public decimal ProductDisposal { get; set; }
        
        public decimal ProductSurplus { get; set; }
        
        public DateTime LastShipmentDate { get; set; }
        
        public DateTime LastSaleDate { get; set; }
        
    }
}