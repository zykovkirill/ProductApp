namespace ProductApp.Shared.Models
{
    public class Rating : Record
    {
        public int ProductRating { get; set; }
        public string UserId { get; set; }
    }
}
