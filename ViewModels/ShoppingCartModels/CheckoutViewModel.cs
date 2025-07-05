using System.ComponentModel.DataAnnotations;

namespace LiquorStore.Web.ViewModels.ShoppingCartModels // Prilagodi namespace
{
    public class CheckoutViewModel
    {
        // Obavezna polja za dostavu
        [Required(ErrorMessage = "Adresa za dostavu je obavezna.")]
        [Display(Name = "Adresa")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Grad za dostavu je obavezna.")]
        [Display(Name = "Grad")]
        public string ShippingCity { get; set; }

        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        [Display(Name = "Poštanski broj")]
        public string ShippingPostalCode { get; set; }

        [Required(ErrorMessage = "Država je obavezna.")]
        [Display(Name = "Država")]
        public string ShippingCountry { get; set; } // Dodaj ako je potrebno

        [EmailAddress]
        [Display(Name = "Email za kontakt")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [Display(Name = "Kontakt Telefon")]
        public string ContactPhoneNumber { get; set; }

        [Required(ErrorMessage = "Kontakt email je obavezan.")]
        [EmailAddress]
        [Display(Name = "Kontakt Email")]
        public string ContactEmail { get; set; }
    }
}