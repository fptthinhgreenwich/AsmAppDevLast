using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
	[Area("JobSeeker")]
	public class ProfileController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProfileController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.GetUserAsync(User);
			return View(currentUser);
		}

		public IActionResult Edit(string? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var userProfile = _unitOfWork.AppUserRepository.Get(user => user.Id == id);
			if (userProfile == null)
			{
				return NotFound();
			}

			return View(userProfile);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, ApplicationUser updatedUser, IFormFile? avatarFile, IFormFile? cvFile)
		{
			if (id != updatedUser.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var user = (ApplicationUser)await _userManager.FindByIdAsync(id);

					// Update user details
					user.Name = updatedUser.Name;
					user.Address = updatedUser.Address;
					user.Introduction = updatedUser.Introduction;

					string rootPath = _webHostEnvironment.WebRootPath;

					// Process avatar file upload
					if (avatarFile != null)
					{
						string avatarFileName = Guid.NewGuid() + Path.GetExtension(avatarFile.FileName);
						string avatarDirectory = Path.Combine(rootPath, @"img\avatars");

						if (!string.IsNullOrEmpty(user.Avatar))
						{
							var oldAvatarPath = Path.Combine(rootPath, user.Avatar.TrimStart('\\'));
							if (System.IO.File.Exists(oldAvatarPath))
							{
								System.IO.File.Delete(oldAvatarPath);
							}
						}

						using (var fileStream = new FileStream(Path.Combine(avatarDirectory, avatarFileName), FileMode.Create))
						{
							avatarFile.CopyTo(fileStream);
						}

						user.Avatar = $@"\img\avatars\{avatarFileName}";
					}

					// Process CV file upload
					if (cvFile != null)
					{
						string cvFileName = Guid.NewGuid() + Path.GetExtension(cvFile.FileName);
						string cvDirectory = Path.Combine(rootPath, @"img\cv");

						if (!string.IsNullOrEmpty(user.CV))
						{
							var oldCVPath = Path.Combine(rootPath, user.CV.TrimStart('\\'));
							if (System.IO.File.Exists(oldCVPath))
							{
								System.IO.File.Delete(oldCVPath);
							}
						}

						using (var fileStream = new FileStream(Path.Combine(cvDirectory, cvFileName), FileMode.Create))
						{
							cvFile.CopyTo(fileStream);
						}

						user.CV = $@"\img\cv\{cvFileName}";
					}

					// Save updated user details
					_unitOfWork.AppUserRepository.Update(user);
					_unitOfWork.Save();

					TempData["success"] = "Profile updated successfully!";
					return RedirectToAction("Index");
				}
				catch (Exception)
				{
					throw;
				}
			}

			return View(updatedUser);
		}
	}
}
