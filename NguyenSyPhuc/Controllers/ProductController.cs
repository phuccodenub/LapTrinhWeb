using Microsoft.AspNetCore.Mvc;
using NguyenSyPhuc.Models; // Thay thế bằng namespace thực tế của bạn
using NguyenSyPhuc.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;


namespace NguyenSyPhuc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: Hiển thị form thêm sản phẩm
        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Xử lý form thêm sản phẩm với ảnh tải lên
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                // Nếu có ảnh đại diện, lưu ảnh vào thư mục
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Nếu có ảnh bổ sung, lưu các ảnh này vào thư mục
                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in imageUrls)
                    {
                        product.ImageUrls.Add(await SaveImage(file));
                    }
                }

                // Thêm sản phẩm vào kho dữ liệu
                _productRepository.Add(product);
                return RedirectToAction("Index");
            }

            // Nếu validation thất bại, tái tạo lại danh sách danh mục và trở lại trang Add
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Lưu ảnh vào thư mục và trả về đường dẫn tương đối
        private async Task<string> SaveImage(IFormFile image)
        {
            // Đảm bảo thư mục tồn tại
            var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }
            
            // Tạo tên file duy nhất để tránh trùng lặp
            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(imagesDir, fileName);

            // Lưu ảnh vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối
            return "/images/" + fileName;
        }

        // Hiển thị danh sách các sản phẩm
        public IActionResult Index()
        {
            var products = _productRepository.GetAll();

            // Nạp thông tin Category cho mỗi sản phẩm
            foreach (var product in products)
            {
                // Tìm Category của sản phẩm từ CategoryId
                product.Category = _categoryRepository.GetAllCategories()
                                                       .FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return View(products);
        }

        // Hiển thị chi tiết của một sản phẩm
        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Nạp thông tin Category cho sản phẩm
            product.Category = _categoryRepository.GetAllCategories()
                                                 .FirstOrDefault(c => c.Id == product.CategoryId);
                                                 
            return View(product);
        }

        // Hiển thị form cập nhật thông tin sản phẩm
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Nạp danh sách danh mục để hiển thị trong dropdown
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            
            return View(product);
        }

        // Xử lý cập nhật thông tin sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                // Lấy sản phẩm hiện tại từ repository để giữ lại các thông tin không thay đổi
                var existingProduct = _productRepository.GetById(product.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }
                
                // Cập nhật thông tin cơ bản
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                
                // Xử lý ảnh đại diện nếu có
                if (imageUrl != null)
                {
                    existingProduct.ImageUrl = await SaveImage(imageUrl);
                }
                
                // Xử lý các ảnh bổ sung nếu có
                if (imageUrls != null && imageUrls.Count > 0)
                {
                    if (existingProduct.ImageUrls == null)
                    {
                        existingProduct.ImageUrls = new List<string>();
                    }
                    
                    foreach (var file in imageUrls)
                    {
                        existingProduct.ImageUrls.Add(await SaveImage(file));
                    }
                }
                
                // Cập nhật sản phẩm
                _productRepository.Update(existingProduct);
                return RedirectToAction("Index");
            }
            
            // Nếu validation thất bại, tái tạo lại danh sách danh mục và trở lại trang Update
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Nạp thông tin Category cho sản phẩm
            product.Category = _categoryRepository.GetAllCategories()
                                                 .FirstOrDefault(c => c.Id == product.CategoryId);
                                                 
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
