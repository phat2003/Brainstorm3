using Brainstorm.Models;
using Brainstorm.Models.ViewModel; // Thêm dòng này để dùng UserRoleVM
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh sách tất cả người dùng
            var users = _userManager.Users.ToList();

            // 2. Tạo một danh sách rỗng để chứa dữ liệu hiển thị (ViewModel)
            var userRoleVMs = new List<UserRoleVM>();

            // 3. Duyệt qua từng người dùng để lấy thêm Role
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userRoleVMs.Add(new UserRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    // Một người có thể có nhiều role, ta ghép chúng lại bằng dấu phẩy
                    Role = string.Join(", ", roles)
                });
            }

            // 4. Trả danh sách này về cho View
            return View(userRoleVMs);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string userId)
        {
            // 1. Tìm người dùng
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 2. Lấy Role hiện tại (Giả sử dự án của bạn mỗi người chỉ có 1 Role tại một thời điểm)
            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault();

            // 3. Lấy tất cả Roles trong hệ thống và chuyển đổi thành dạng danh sách thả xuống (SelectListItem)
            var roles = _roleManager.Roles.ToList();
            var roleList = roles.Select(x => new SelectListItem
            {
                Text = x.Name, // Chữ hiển thị cho Admin thấy (VD: "Staff")
                Value = x.Name // Giá trị thực sự gửi về Server
            });

            // 4. Đóng gói tất cả vào ViewModel
            var roleVM = new RoleManagementVM
            {
                User = user,
                OldRole = oldRole,
                RoleList = roleList
            };

            // 5. Gửi ra View
            return View(roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, string newRole)
        {
            // Kiểm tra xem dữ liệu gửi lên có bị trống không
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                return RedirectToAction("Index");
            }

            // 1. Tìm người dùng trong cơ sở dữ liệu
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 2. Lấy danh sách các Vai trò (Role) cũ của người dùng này và xóa chúng đi
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            // 3. Cấp Vai trò mới mà Admin vừa chọn
            await _userManager.AddToRoleAsync(user, newRole);

            // 4. Quay trở về trang danh sách
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            // 1. Tìm người dùng
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 2. Lấy Role hiện tại (Giả sử dự án của bạn mỗi người chỉ có 1 Role tại một thời điểm)
            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault();

            // 3. Lấy tất cả Roles trong hệ thống và chuyển đổi thành dạng danh sách thả xuống (SelectListItem)
            var roles = _roleManager.Roles.ToList();
            var roleList = roles.Select(x => new SelectListItem
            {
                Text = x.Name, // Chữ hiển thị cho Admin thấy (VD: "Staff")
                Value = x.Name // Giá trị thực sự gửi về Server
            });

            // 4. Đóng gói tất cả vào ViewModel
            var roleVM = new RoleManagementVM
            {
                User = user,
                OldRole = oldRole,
                RoleList = roleList
            };

            // 5. Gửi ra View
            return View(roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
            return View();
        }

    }
}