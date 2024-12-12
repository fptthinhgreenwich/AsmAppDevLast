using AsmAppDev.Models.ViewModels;
using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
	[Area("JobSeeker")]
    [Authorize(Roles = "JobSeeker")]

    public class ViewJobController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public ViewJobController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

        // Displays the list of jobs with search functionality
        public IActionResult Index(string search, string sortOrder)
        {
            // Lấy danh sách công việc từ database
            var jobList = _unitOfWork.JobRepository.GetAll("Category").AsQueryable();

            // Nếu có từ khóa tìm kiếm, lọc theo các tiêu chí
            if (!string.IsNullOrEmpty(search))
            {
                jobList = jobList.Where(j =>
                    j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || // Tìm trong tiêu đề
                    j.Description.Contains(search, StringComparison.OrdinalIgnoreCase) || // Tìm trong mô tả
                    j.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase) // Tìm trong danh mục
                );
            }

            // Sắp xếp theo RequiredQualification
            switch (sortOrder)
            {
                case "required_yes":
                    jobList = jobList.Where(j => j.requiredQualification.Equals("Yes", StringComparison.OrdinalIgnoreCase));
                    break;
                case "required_no":
                    jobList = jobList.Where(j => j.requiredQualification.Equals("No", StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    break; // Không sắp xếp
            }

            // Truyền giá trị tìm kiếm và sắp xếp vào View
            ViewData["SearchValue"] = search;
            ViewData["CurrentSort"] = sortOrder;

            return View(jobList.ToList());
        }

        // Display the job application page
        public IActionResult Apply(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var job = _unitOfWork.JobRepository.Get(j => j.Id == id);
			if (job == null)
			{
				return NotFound();
			}

			var jobViewModel = new JobVM
			{
				apply = new JobApplication { JobId = job.Id },
				Job = job
			};

			return View(jobViewModel);
		}

		// Handles job application submission
		[HttpPost]
		public async Task<IActionResult> Apply(JobVM jobViewModel)
		{
			if (ModelState.IsValid)
			{
				var currentUser = await _userManager.GetUserAsync(User);

				if (currentUser == null)
				{
					return RedirectToAction("Login", "Account");
				}

				// Set applicant details
				jobViewModel.apply.Email = currentUser.Email;
				jobViewModel.apply.DayApply = DateTime.Now;

				// Save application to the database
				_unitOfWork.JobApplicationRepository.Add(jobViewModel.apply);
				_unitOfWork.Save();

				TempData["success"] = "Job applied successfully!";
				return RedirectToAction("Index");
			}

			return View(jobViewModel);
		}
	}
}
