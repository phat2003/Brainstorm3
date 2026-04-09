using System.ComponentModel.DataAnnotations;

namespace Brainstorm.Models.ViewModel
{
    public class AdminResetPasswordVM
    {
        [Required]
        public string UserId { get; set; }

        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
