using System.ComponentModel.DataAnnotations;

public class LoginUser
{
    [Required]
    [Display(Name = "Username")]
    public string Email {get; set;}
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}