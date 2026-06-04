namespace Radiocab.Models
{
    public class ListingViewModel
    {
        public Listing NewListing { get; set; } = new Listing();
        public List<Listing> AllListings { get; set; } = new List<Listing>();
    }
}
