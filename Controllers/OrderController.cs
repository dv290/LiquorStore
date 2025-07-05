using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LiquorStore.DAL;
using LiquorStore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace LiquorStore.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly LiquorStoreDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public OrderController(LiquorStoreDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                                       .Include(o => o.User)
                                       .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                                        .Include(o => o.User)
                                        .Include(o => o.OrderItems)
                                            .ThenInclude(oi => oi.Product)
                                        .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,TotalAmount,OrderStatus,UserId,ShippingAddress,ShippingCity,ShippingPostalCode,ShippingCountry,ContactPhoneNumber,ContactEmail")] Order order) // DODANA NOVA POLJA U BIND
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var originalOrder = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

                    if (originalOrder == null)
                    {
                        return NotFound();
                    }

                    originalOrder.OrderStatus = order.OrderStatus;
                    originalOrder.ShippingAddress = order.ShippingAddress;
                    originalOrder.ShippingCity = order.ShippingCity;
                    originalOrder.ShippingPostalCode = order.ShippingPostalCode;
                    originalOrder.ShippingCountry = order.ShippingCountry; 
                    originalOrder.ContactPhoneNumber = order.ContactPhoneNumber; 
                    originalOrder.ContactEmail = order.ContactEmail; 

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Create()
        {
            ViewBag.UserId = new SelectList(_userManager.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,TotalAmount,OrderStatus,UserId,ShippingAddress,ShippingCity,ShippingPostalCode,ShippingCountry,ContactPhoneNumber,ContactEmail")] Order order)
        {
            order.OrderDate = DateTime.Now;


            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("UserId", "Odabrani korisnik ne postoji.");
                    ViewBag.UserId = new SelectList(_userManager.Users, "Id", "UserName", order.UserId);
                    return View(order);
                }
                order.User = user; 

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = new SelectList(_userManager.Users, "Id", "UserName", order.UserId);
            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}