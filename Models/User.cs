using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace OneProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "First name must be 2 characters or longer!")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Last name must be 2 characters or longer!")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Usename must be 3 characters or longer!")]
        [MaxLength(20, ErrorMessage = "Usename must be 20 characters maximum!")]
        [Display(Name = "Username")]
        public string EmailReg { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        [Display(Name = "Password")]
        public string PasswordReg { get; set; }
        public double Wallet { get; set; } = 1000;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Association> Auctions { get; set; }
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("PasswordReg")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Required]
        public string Confirm { get; set; }
    }
}