namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class Hotel
    {
        public Guid HotelId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public Decimal Cost { get; set; }
        public int AvailableSpaces { get; set; }
    }
}
