namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class HotelBooking
    {
        public Guid HotelBookingId { get; set; }
        public string UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public bool IsCancelled { get; set; } = false;

        // Navigation properties
        public Hotel Hotel { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
