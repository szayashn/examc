namespace OneProject.Models
{
    public class Association
    {
        public int AssociationId { get; set; }
        public int AuctionId { get; set; }
        public int UserId { get; set; }
        public Auction Auction { get; set; }
        public User User { get; set; }
        
    }
}