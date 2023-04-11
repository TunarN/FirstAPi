﻿namespace FirstApi.Models
{
    public class Product:BaseEntity
    {
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public string Name { get; set; }
        public double SalePrice { get; set; }
        public double CostPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
