using AsmAppDev.Models;
using AsmAppDev.Models.ViewModels;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AsmAppDev.Areas.Users.Controllers
{
	[Area("Employer")]
    [Authorize(Roles = "Employer")] // Chỉ cho phép người có vai trò Employer

    public class JobController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public JobController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return View(new List<Job>());

			var jobs = _unitOfWork.JobRepository.GetAll("Category")
				.Where(j => j.UserId == userId)
				.ToList();

			return View(jobs);
		}

		public IActionResult Create()
		{
			var jobVM = new JobVM
			{
				Categories = _unitOfWork.CategoryRepository.GetAll()
					.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
					{
						Text = c.Name,
						Value = c.Id.ToString()
					}),
				Job = new Job()
			};
			return View(jobVM);
		}

		[HttpPost]
		public IActionResult Create(JobVM jobVM)
		{
			if (!ModelState.IsValid) return View(jobVM);

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return View(jobVM);

			jobVM.Job.UserId = userId;
			_unitOfWork.JobRepository.Add(jobVM.Job);
			_unitOfWork.Save();

			TempData["success"] = "Job created successfully!";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var jobVM = new JobVM
			{
				Categories = _unitOfWork.CategoryRepository.GetAll()
					.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
					{
						Text = c.Name,
						Value = c.Id.ToString()
					}),
				Job = _unitOfWork.JobRepository.Get(c => c.Id == id)
			};

			return jobVM.Job == null ? NotFound() : View(jobVM);
		}

		[HttpPost]
		public IActionResult Edit(JobVM jobVM)
		{
			if (!ModelState.IsValid) return View(jobVM);

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return View(jobVM);

			jobVM.Job.UserId = userId;
			_unitOfWork.JobRepository.Update(jobVM.Job);
			_unitOfWork.Save();

			TempData["success"] = "Job updated successfully!";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var job = _unitOfWork.JobRepository.Get(c => c.Id == id, "Category");
			return job == null ? NotFound() : View(job);
		}

		[HttpPost]
		public IActionResult Delete(Job job)
		{
			_unitOfWork.JobRepository.Delete(job);
			_unitOfWork.Save();

			TempData["success"] = "Job deleted successfully!";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult ViewJobApp(int? id, string sortBy, string filterBy)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Expression<Func<JobApplication, bool>> filter = j => j.JobId == id;
			var jobApps = _unitOfWork.JobApplicationRepository.GetAllJobApp(filter);

			if (!string.IsNullOrEmpty(sortBy))
			{
				jobApps = sortBy switch
				{
					"email" => jobApps.OrderBy(j => j.Email),
					"emailDesc" => jobApps.OrderByDescending(j => j.Email),
					"dayApply" => jobApps.OrderBy(j => j.DayApply),
					"dayApplyDesc" => jobApps.OrderByDescending(j => j.DayApply),
					_ => jobApps
				};
			}

			if (!string.IsNullOrEmpty(filterBy))
			{
				jobApps = jobApps.Where(j => j.Email.Contains(filterBy));
			}

			return View(jobApps);
		}


		public async Task<IActionResult> ViewProfile(int? id)
		{
			if (id == null)
				return NotFound();

			var jobApplication = _unitOfWork.JobApplicationRepository.Get(c => c.Id == id);
			if (jobApplication == null)
				return NotFound();

			var jobSeeker = await _userManager.FindByEmailAsync(jobApplication.Email);
			return jobSeeker == null ? NotFound() : View(jobSeeker);
		}

		public IActionResult Accept(int? id)
		{
			if (id == null)
				return NotFound();

			var jobApp = _unitOfWork.JobApplicationRepository.Get(c => c.Id == id);
			if (jobApp == null)
				return NotFound();

			jobApp.Status = true;
			_unitOfWork.JobApplicationRepository.Update(jobApp);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Decline(int? id)
		{
			if (id == null)
				return NotFound();

			var jobApp = _unitOfWork.JobApplicationRepository.Get(c => c.Id == id);
			if (jobApp == null)
				return NotFound();

			_unitOfWork.JobApplicationRepository.Delete(jobApp);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}
	}
}
