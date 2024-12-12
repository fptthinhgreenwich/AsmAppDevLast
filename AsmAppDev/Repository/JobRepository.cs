using AsmAppDev.Data;
using AsmAppDev.Models;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Repository.IRepository
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public JobRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Job entity)
        {
            _dbContext.Jobs.Update(entity);
        }
    }
}
