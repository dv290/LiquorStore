using System.ComponentModel.DataAnnotations;

namespace LiquorStore.Web.ViewModels.UserModels
{
    public class UserEditViewModel
    {
        [Required] 
        public int UserId { get; set; }

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public required string LastName { get; set; }


        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova lozinka mora imati najmanje 6 znakova.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova lozinka")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi novu lozinku")]
        [Compare("NewPassword", ErrorMessage = "Nova lozinka i potvrda se ne podudaraju.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
