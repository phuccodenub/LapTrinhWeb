using System.ComponentModel.DataAnnotations;
namespace NguyenSyPhuc.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //Kết nối với bảng Product
        public List<Product>? Products { get; set; }
    }
}
