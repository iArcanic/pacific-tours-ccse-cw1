namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class HotelAvailability
    {
        public Guid HotelAvailabilityId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        // Navigation properties
        public Hotel Hotel { get; set; }
    }
}
