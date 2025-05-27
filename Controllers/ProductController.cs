using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NguyenSyPhuc.Models;
using NguyenSyPhuc.Repositories;

namespace NguyenSyPhuc.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var list_Product = await _productRepository.GetAll();
            return View(list_Product);
        }

        //Tạo form để điền dữ liệu
        public async Task<IActionResult> Create()
        {
            var list_Category = await _categoryRepository.GetAll();
            ViewBag.Categories = new SelectList(list_Category, "Id", "Name");

            return View("Add");
        }

        //Khi nhấn OK để tạo sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? file_Image)
        {
            if (ModelState.IsValid)
            {
                //Lưu hình ảnh
                if (file_Image != null && file_Image.Length > 0)
                {
                    string path = "wwwroot/images";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filename = Guid.NewGuid() + Path.GetExtension(file_Image.FileName);
                    path = Path.Combine(path, filename);
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        await file_Image.CopyToAsync(fs);
                    }

                    product.ImageUrl = "images/" + filename;
                }

                //Lưu CSDL
                await _productRepository.Create(product);
                return RedirectToAction("Index", "Product");
            }

            return View("Add", product);
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Tạo form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            
            var list_Category = await _categoryRepository.GetAll();
            ViewBag.Categories = new SelectList(list_Category, "Id", "Name", product.CategoryId);
            
            return View(product);
        }
        
        // Lưu thông tin cập nhật
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? imageUrl)
        {
            if (ModelState.IsValid)
            {
                // Lấy sản phẩm hiện tại từ database
                var existingProduct = await _productRepository.GetById(product.Id);
                if (existingProduct == null)
                {
                    TempData["Message"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction("Index");
                }
                
                // Cập nhật thông tin sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                
                // Cập nhật hình ảnh nếu có
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        string oldImagePath = Path.Combine("wwwroot", existingProduct.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    // Lưu ảnh mới
                    string path = "wwwroot/images";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filename = Guid.NewGuid() + Path.GetExtension(imageUrl.FileName);
                    path = Path.Combine(path, filename);
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        await imageUrl.CopyToAsync(fs);
                    }

                    existingProduct.ImageUrl = "images/" + filename;
                }
                
                // Lưu vào CSDL
                await _productRepository.Update(existingProduct);
                TempData["Message"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }
            
            var list_Category = await _categoryRepository.GetAll();
            ViewBag.Categories = new SelectList(list_Category, "Id", "Name", product.CategoryId);
            return View(product);
        }
        
        // Chuyển hướng Edit đến Update để tương thích với các link trong Index
        public async Task<IActionResult> Edit(int id)
        {
            return await Update(id);
        }
        
        // Hiển thị form xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            return View(product);
        }
        
        // Xác nhận xóa sản phẩm
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            
            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine("wwwroot", product.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            
            await _productRepository.Delete(id);
            TempData["Message"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Index");
        }
    }
}
