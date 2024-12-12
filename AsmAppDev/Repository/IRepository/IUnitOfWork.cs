using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IJobRepository JobRepository { get; }
        IAppUserRepository AppUserRepository { get; }
        IJobApplicationRepository JobApplicationRepository { get; }
        void Save();
    }
}
