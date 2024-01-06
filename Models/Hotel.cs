namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class Hotel
    {
        public Guid HotelId { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public decimal Cost { get; set; }
        public int AvailableSpaces { get; set; }
    }
}
