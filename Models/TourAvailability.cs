namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class TourAvailability
    {
        public Guid TourAvailabilityId { get; set; }
        public Guid TourId { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        // Navigation properties
        public Tour Tour { get; set; }
    }
}
