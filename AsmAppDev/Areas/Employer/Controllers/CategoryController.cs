using AsmAppDev.Models;
using AsmAppDev.Models.ViewModels;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AsmAppDev.Areas.Users.Controllers
{
	[Area("Employer")]
	[Authorize(Roles = "Employer")]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (!string.IsNullOrEmpty(userId))
			{
				var categories = _unitOfWork.CategoryRepository.GetAll()
					.Where(c => c.UserId == userId)
					.ToList();
				return View(categories);
			}

			return View(new List<Category>());
		}

		public async Task<IActionResult> ToggleNotification(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}

			var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			// Toggle notification status
			category.NotificationStatus = !category.NotificationStatus;

			_unitOfWork.Save();

			TempData["Success"] = "Notification status updated successfully!";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category category)
		{
			if (ModelState.IsValid)
			{
				var claimIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (!string.IsNullOrEmpty(userId))
				{
					category.UserId = userId;
					category.DateCreate = DateTime.Now;

					_unitOfWork.CategoryRepository.Add(category);
					_unitOfWork.Save();

					TempData["Success"] = "Category created successfully!";
				}

				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				var claimIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (!string.IsNullOrEmpty(userId))
				{
					category.UserId = userId;
					category.DateCreate = DateTime.Now;

					_unitOfWork.CategoryRepository.Update(category);
					_unitOfWork.Save();

					TempData["Success"] = "Category updated successfully!";
				}

				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

		[HttpPost]
		public IActionResult Delete(Category category)
		{
			if (category == null)
			{
				return NotFound();
			}

			_unitOfWork.CategoryRepository.Delete(category);
			_unitOfWork.Save();

			TempData["Success"] = "Category deleted successfully!";
			return RedirectToAction(nameof(Index));
		}
	}
}
