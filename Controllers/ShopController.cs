using LiquorStore.DAL;
using LiquorStore.Model;
using LiquorStore.Web.ViewModels.ShoppingCartModels; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LiquorStore.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly LiquorStoreDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(LiquorStoreDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var liquorStoreDbContext = _context.Products
                                               .Include(p => p.Category)
                                               .Include(p => p.Manufacturer);
            return View(await liquorStoreDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [Authorize] 
        public async Task<IActionResult> AddToCart(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new ShoppingCartItem
                {
                    ProductId = product.ProductId,
                    Product = product, 
                    Quantity = 1,
                    Price = product.Price 
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["Message"] = $"Proizvod '{product.Name}' dodan u košaricu. Trenutno u košarici: {cart.Sum(item => item.Quantity)} stavki.";

            return RedirectToAction(nameof(Cart));
        }

        [Authorize] 
        public IActionResult Cart()
        {
            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();
            return View(cart);
        }

        [Authorize] 
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart");
            if (cart == null)
            {
                return RedirectToAction(nameof(Cart));
            }

            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == id);
            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity > 1)
                {
                    itemToRemove.Quantity--; 
                }
                else
                {
                    cart.Remove(itemToRemove); 
                }
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Message"] = $"Količina proizvoda '{itemToRemove.Product.Name}' smanjena.";
            }

            return RedirectToAction(nameof(Cart));
        }

        [Authorize] 
        public IActionResult ClearItemFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart");
            if (cart == null)
            {
                return RedirectToAction(nameof(Cart));
            }

            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Message"] = $"Proizvod '{itemToRemove.Product.Name}' uklonjen iz košarice.";
            }

            return RedirectToAction(nameof(Cart));
        }

        
        
        [Authorize] 
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Message"] = "Košarica je ispražnjena.";
            return RedirectToAction(nameof(Index)); 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            if (!cart.Any())
            {
                TempData["Message"] = "Vaša košarica je prazna. Dodajte proizvode prije odjave.";
                return RedirectToAction(nameof(Cart));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                Email = currentUser?.Email,
                ContactPhoneNumber = "",
                ContactEmail = currentUser?.Email
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            List<ShoppingCartItem> cart = HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>("Cart") ?? new List<ShoppingCartItem>();

            if (!cart.Any())
            {
                TempData["Message"] = "Vaša košarica je prazna. Ne možete završiti narudžbu.";
                return RedirectToAction(nameof(Cart));
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var order = new Order
                {
                    UserId = currentUser.Id,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = model.ShippingAddress,
                    ShippingCity = model.ShippingCity,
                    ShippingPostalCode = model.ShippingPostalCode,
                    ShippingCountry = model.ShippingCountry,
                    OrderStatus = "Pending",
                    ContactPhoneNumber = model.ContactPhoneNumber,
                    ContactEmail = model.ContactEmail,
                    TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                    OrderItems = cart.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");
                TempData["SuccessMessage"] = "Vaša narudžba je uspješno poslana! Hvala vam na kupnji.";

                return RedirectToAction("OrderConfirmation", "Shop", new { orderId = order.OrderId });
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                                    .Include(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Product)
                                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == _userManager.GetUserId(User));
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

    }
}