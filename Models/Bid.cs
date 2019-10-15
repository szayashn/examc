using System.ComponentModel.DataAnnotations;

public class Bid
{
    [Required]
    public double CurrentBid {get; set;}
    public int Id { get; set; }
}