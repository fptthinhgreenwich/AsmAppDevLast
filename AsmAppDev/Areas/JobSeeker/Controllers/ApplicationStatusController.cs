using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class ApplicationStatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationStatusController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userEmail = currentUser.Email;

            var jobApplications = _unitOfWork.JobApplicationRepository
                .GetAll("Job")
                .Where(application => application.Email == userEmail)
                .ToList();

            return View(jobApplications);
        }
    }
}
