namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class Tour
    {
        public Guid TourId { get; set; }
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public decimal Cost { get; set; }
        public int AvailableSpaces { get; set; }
    }
}
