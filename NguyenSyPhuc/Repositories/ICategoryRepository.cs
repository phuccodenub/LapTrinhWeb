using NguyenSyPhuc.Models;

namespace NguyenSyPhuc.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}

