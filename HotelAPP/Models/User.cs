namespace HotelAPP.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PNumber { get; set; }
        public bool IsAdmin { get; set; }
    }
}
