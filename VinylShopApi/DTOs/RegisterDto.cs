namespace VinylShopApi.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // domyślnie User, ale można przekazać "Admin"
    }
}