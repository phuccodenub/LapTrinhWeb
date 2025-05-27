namespace NguyenSyPhuc.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public string Url { get; set; }

        //Kết nối bảng Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
