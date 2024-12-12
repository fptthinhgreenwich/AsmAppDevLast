using AsmAppDev.Data;
using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using System.Linq.Expressions;

namespace AsmAppDev.Repository
{
    public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public JobApplicationRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<JobApplication> GetAllJobApp(Expression<Func<JobApplication, bool>> filter = null)
        {
            IQueryable<JobApplication> query = _dbContext.JobApplications;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

        public void Update(JobApplication entity)
        {
            _dbContext.JobApplications.Update(entity);
        }
    }
}
