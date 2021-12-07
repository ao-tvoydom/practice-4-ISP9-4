using  System;

namespace Infrastructure
{
    public class DepartmentProduct
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        
        public int ProductId { get; set; }
        
        public decimal Realization { get; set; }
        
        public decimal ProductDisposal { get; set; }
        
        public decimal ProductSurplus { get; set; }
        
        public DateOnly LastShipmentDate { get; set; }
        
        public DateOnly LastSaleDate { get; set; }
        
    }
}