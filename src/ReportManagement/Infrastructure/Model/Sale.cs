using System;

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
        
        public int BlockStatusID { get; set; }
        
        public DateTime LastShipmentDate { get; set; }
        
        public DateTime LastSaleDate { get; set; }
        
        public decimal Disposal { get; set; }
    }
}