namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class TourBooking
    {
        public Guid TourBookingId { get; set; }
        public string UserId { get; set; }
        public Guid TourId { get; set; }
        public DateTime TourStartDate { get; set; }
        public DateTime TourEndDate { get; set; }

        // Navigation properties
        public Tour Tour { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
