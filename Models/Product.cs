using System.ComponentModel.DataAnnotations;

namespace NguyenSyPhuc.Models
{
    public class Product
    {
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Tên SP không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Range(0, 10000000, ErrorMessage = "Giá SP nằm từ 0 - 10000000")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        //Kết nối bảng Category
        //(Category là khóa chính - Product là khóa phụ)
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        //Kết nối bảng ProductImage
        //(ProductImage là khóa phụ - Product là khóa chính)
        public List<ProductImage>? Images { get; set; }
    }
}
