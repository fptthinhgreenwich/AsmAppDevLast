using AsmAppDev.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category entity);
    }
}
