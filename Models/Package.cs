namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class Package
    {
        public Guid PackageId { get; set; }
        public Guid HotelId { get; set; }
        public Guid TourId { get; set; }
        public decimal Cost { get; set; }

        // Navigation properties
        public Tour Tour { get; set; }
        public Hotel Hotel { get; set; }
    }
}
