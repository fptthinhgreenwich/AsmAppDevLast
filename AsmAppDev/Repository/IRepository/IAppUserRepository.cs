using AsmAppDev.Models;
using AsmAppDev.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Repository.IRepository
{
    public interface IAppUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser entity);
    }
}
