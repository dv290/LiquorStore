using LiquorStore.Model; 

namespace LiquorStore.Web.ViewModels.ShoppingCartModels
{
    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public required Product Product { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
    }
}