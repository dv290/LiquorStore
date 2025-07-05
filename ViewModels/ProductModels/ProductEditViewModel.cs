using System.ComponentModel.DataAnnotations;

namespace LiquorStore.Web.ViewModels.ProductModels
{
    public class ProductEditViewModel
    {
        // ProductId je potreban za identifikaciju proizvoda koji se uređuje
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Naziv proizvoda je obavezan.")]
        [StringLength(200, ErrorMessage = "Naziv proizvoda ne smije biti dulji od 200 znakova.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Opis proizvoda ne smije biti dulji od 1000 znakova.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, 10000.00, ErrorMessage = "Cijena mora biti između 0.01 i 10000.00.")]
        public decimal Price { get; set; }

        // Trenutna putanja do slike (za prikaz u formi, nije za upload)
        public string? CurrentImageUrl { get; set; }

        // Svojstvo za upload nove datoteke (nije obavezno, jer korisnik možda ne želi mijenjati sliku)
        [Display(Name = "Odaberi novu sliku (ostavi prazno za zadržavanje postojeće)")]
        public IFormFile? NewImageFile { get; set; } // Promijenio sam naziv da bude jasnije

        [Required(ErrorMessage = "Sadržaj alkohola je obavezan.")]
        [Range(0.0, 100.0, ErrorMessage = "Sadržaj alkohola mora biti između 0.0 i 100.0.")]
        public double AlcoholContent { get; set; }

        [Required(ErrorMessage = "Volumen je obavezan.")]
        [Range(1, 10000, ErrorMessage = "Volumen mora biti između 1 i 10000 ml.")]
        public int VolumeMl { get; set; }

        [Required(ErrorMessage = "Količina na zalihi je obavezna.")]
        [Range(0, int.MaxValue, ErrorMessage = "Količina na zalihi mora biti pozitivan broj.")]
        public int StockQuantity { get; set; }

        // Strani ključevi
        [Required(ErrorMessage = "Kategorija je obavezna.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Proizvođač je obavezan.")]
        public int ManufacturerId { get; set; }
    }
}
