using LiquorStore.DAL;
using LiquorStore.Model;
using LiquorStore.Web.ViewModels.ProductModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LiquorStore.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly LiquorStoreDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(LiquorStoreDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                              .Include(p => p.Category) 
                              .Include(p => p.Manufacturer) 
                              .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.ManufacturerId = new SelectList(await _context.Manufacturers.ToListAsync(), "ManufacturerId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                //upload slike 
                string imageUrl = ""; 
                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }
                    imageUrl = "/images/" + fileName;
                }

                var product = new Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    ImageUrl = imageUrl, 
                    AlcoholContent = viewModel.AlcoholContent,
                    VolumeMl = viewModel.VolumeMl,
                    StockQuantity = viewModel.StockQuantity,
                    CategoryId = viewModel.CategoryId,
                    ManufacturerId = viewModel.ManufacturerId,
                    CreatedDate = DateTime.Now 
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", viewModel.CategoryId);
            ViewBag.ManufacturerId = new SelectList(await _context.Manufacturers.ToListAsync(), "ManufacturerId", "Name", viewModel.ManufacturerId);
            return View(viewModel); 
        }

        public async Task<IActionResult> Edit(int? id)
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

            var viewModel = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CurrentImageUrl = product.ImageUrl, 
                AlcoholContent = product.AlcoholContent,
                VolumeMl = product.VolumeMl,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ManufacturerId = product.ManufacturerId
            };


            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
            ViewBag.ManufacturerId = new SelectList(await _context.Manufacturers.ToListAsync(), "ManufacturerId", "Name", product.ManufacturerId);

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var productToUpdate = await _context.Products.FindAsync(id);

                if (productToUpdate == null)
                {
                    return NotFound();
                }

                if (viewModel.NewImageFile != null && viewModel.NewImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(productToUpdate.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productToUpdate.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await viewModel.NewImageFile.CopyToAsync(fileStream);
                    }
                    productToUpdate.ImageUrl = "/images/" + fileName; 
                }

                productToUpdate.Name = viewModel.Name;
                productToUpdate.Description = viewModel.Description;
                productToUpdate.Price = viewModel.Price;
                productToUpdate.AlcoholContent = viewModel.AlcoholContent;
                productToUpdate.VolumeMl = viewModel.VolumeMl;
                productToUpdate.StockQuantity = viewModel.StockQuantity;
                productToUpdate.CategoryId = viewModel.CategoryId;
                productToUpdate.ManufacturerId = viewModel.ManufacturerId;
                productToUpdate.LastUpdatedDate = DateTime.Now; 

                try
                {
                    _context.Update(productToUpdate); 
                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productToUpdate.ProductId))
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


            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", viewModel.CategoryId);
            ViewBag.ManufacturerId = new SelectList(await _context.Manufacturers.ToListAsync(), "ManufacturerId", "Name", viewModel.ManufacturerId);
            return View(viewModel);
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

        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); 
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); 
                }
            }

            _context.Products.Remove(product); 
            await _context.SaveChangesAsync(); 

            return RedirectToAction(nameof(Index)); 
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
