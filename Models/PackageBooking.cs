namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class PackageBooking
    {
        public Guid PackageBookingId { get; set; }
        public string UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Guid TourId { get; set; }
        public DateTime TourStartDate { get; set; }
        public DateTime TourEndDate { get; set; }
        public bool IsCancelled { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        // Navigation properties
        public Hotel Hotel { get; set; }
        public Tour Tour { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
