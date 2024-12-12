using AsmAppDev.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Repository.IRepository
{
    public interface IJobRepository : IRepository<Job>
    {
        void Update(Job entity);
    }
}
