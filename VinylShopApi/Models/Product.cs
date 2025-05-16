namespace VinylShopApi.Models
{
    public class Product
    {
        public int Id { get; set; } // Klucz główny
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
