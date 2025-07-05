using System.ComponentModel.DataAnnotations;

namespace LiquorStore.Web.ViewModels.UserModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova.")]
        [DataType(DataType.Password)] 
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno.")]
        public required string LastName { get; set; }
    }
}
