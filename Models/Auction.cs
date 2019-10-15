using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace OneProject.Models
{
    public class Auction
    {
        public int AuctionId { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Product name must be 3 characters or longer!")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Description must be 10 characters or longer!")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Range(0, 9999999999)]
        [Display(Name = "Starting bid")]
        public double Bid { get; set; }
        public string BidderId { get; set; }
        [Required]
        [Display(Name = "End day")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;
        public List<Association> Users { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}