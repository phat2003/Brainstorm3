using Brainstorm.DataAccess.Data;
using Brainstorm.Models;
using Brainstorm.Models.ViewModel;
using Brainstorm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UserController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var appUsers = await _db.ApplicationUsers
                .Include(u => u.Department)
                .ToListAsync();
            var appUserMap = appUsers.ToDictionary(u => u.Id, u => u);

            var userRoleVMs = new List<UserRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(SD.Role_User_Admin))
                {
                    continue;
                }

                string departmentName = "-";
                if (appUserMap.TryGetValue(user.Id, out var appUser))
                {
                    departmentName = appUser.Department?.Name ?? "-";
                }

                userRoleVMs.Add(new UserRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Role = string.Join(", ", roles),
                    DepartmentName = departmentName
                });
            }

            return View(userRoleVMs);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var appUser = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);

            if (await IsAdminUser(user))
            {
                TempData["error"] = "Admin accounts cannot be modified here.";
                return RedirectToAction(nameof(Index));
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault();

            var roleList = _roleManager.Roles
                .Select(x => x.Name)
                .Where(r => r != SD.Role_User_Admin)
                .Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r,
                    Selected = r == oldRole
                });

            var departmentList = _db.Departments
                .Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = d.Id.ToString(),
                    Selected = appUser != null && appUser.DepartmentId == d.Id
                })
                .ToList();

            var roleVM = new RoleManagementVM
            {
                User = user,
                OldRole = oldRole,
                DepartmentId = appUser?.DepartmentId,
                RoleList = roleList,
                DepartmentList = departmentList
            };

            return View(roleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string userId, string newRole, int? departmentId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var appUser = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);

            if (await IsAdminUser(user) || newRole == SD.Role_User_Admin)
            {
                TempData["error"] = "Admin role cannot be changed.";
                return RedirectToAction(nameof(Index));
            }

            if (newRole != SD.Role_User_Admin)
            {
                if (!departmentId.HasValue || !await _db.Departments.AnyAsync(d => d.Id == departmentId.Value))
                {
                    TempData["error"] = "Please select a valid department.";
                    return RedirectToAction(nameof(EditRole), new { userId });
                }

                if (appUser != null)
                {
                    appUser.DepartmentId = departmentId;
                    _db.ApplicationUsers.Update(appUser);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    TempData["error"] = "Cannot update department for legacy users. Re-create the user to assign a department.";
                    return RedirectToAction(nameof(EditRole), new { userId });
                }
            }
            else if (appUser != null)
            {
                appUser.DepartmentId = null;
                _db.ApplicationUsers.Update(appUser);
                await _db.SaveChangesAsync();
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["success"] = $"Updated role and department for {user.UserName}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (await IsAdminUser(user))
            {
                TempData["error"] = "Admin accounts cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault();

            var roleVM = new RoleManagementVM
            {
                User = user,
                OldRole = oldRole,
                RoleList = Enumerable.Empty<SelectListItem>()
            };

            return View(roleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (await IsAdminUser(user))
            {
                TempData["error"] = "Admin accounts cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            TempData["success"] = $"Deleted user {user.UserName}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (await IsAdminUser(user))
            {
                TempData["error"] = "Admin passwords cannot be reset from this screen.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new AdminResetPasswordVM
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(AdminResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
            {
                return NotFound();
            }

            if (await IsAdminUser(user))
            {
                TempData["error"] = "Admin passwords cannot be reset from this screen.";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                vm.UserName = user.UserName;
                return View(vm);
            }

            TempData["success"] = $"Password reset successfully for {user.UserName}.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsAdminUser(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains(SD.Role_User_Admin);
        }
    }
}