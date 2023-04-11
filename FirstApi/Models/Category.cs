namespace FirstApi.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public string ImageUrl { get; set; }
        public List<Product> Products { get; set; }
    }
}
