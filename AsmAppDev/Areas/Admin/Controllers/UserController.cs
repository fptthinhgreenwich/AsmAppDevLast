using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace AsmAppDev.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager,  RoleManager<IdentityRole> roleManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
            _roleManager = roleManager;

        }

		public IActionResult Index()
		{
			var userList = _unitOfWork.AppUserRepository.GetAll().ToList();
			return View(userList);
		}

		public async Task<IActionResult> ToggleStatus(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			// Update the status
			user.Status = !user.Status;

			// Persist changes
			_unitOfWork.Save();

			TempData["Success"] = user.Status
				? "Account enabled successfully!"
				: "Account disabled successfully!";

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(string id)
		{
			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			_unitOfWork.AppUserRepository.Delete(user);
			_unitOfWork.Save();

			TempData["Success"] = "Account deleted successfully!";
			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(r => new RoleSelection
                {
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name) 
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model, string Roles)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Where(r => r != Roles).ToList();
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }
            if (!currentRoles.Contains(Roles))
            {
                await _userManager.AddToRoleAsync(user, Roles);
            }

            TempData["Success"] = "Role updated successfully!";
            return RedirectToAction("Index");
        }

    }

}
