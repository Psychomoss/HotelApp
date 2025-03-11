namespace HotelAPP.Models
{
    public class ResvDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int RoomNumber { get; set; }
        public float RoomPrice { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime ExitTime { get; set; }
    }
}
