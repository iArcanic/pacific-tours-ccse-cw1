namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class HotelDiscount
    {
        public Guid HotelDiscountId { get; set; }
        public String RoomType { get; set; }
        public decimal HotelDiscountPercentage { get; set; }
    }
}
