using NguyenSyPhuc.Models;

namespace NguyenSyPhuc.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetById(int id);  // Lấy sản phẩm theo ID
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);

    }
}
