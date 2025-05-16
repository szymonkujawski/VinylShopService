namespace VinylShopApi.Models
{
    public class CartItem
    {
        public int Id { get; set; } // To jest klucz główny
        public int ProductId { get; set; }
        public string Username { get; set; } = string.Empty; // Lub UserId, zależnie od modelu
        public int Quantity { get; set; }
    }
}
