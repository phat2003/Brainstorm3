using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Brainstorm.Models.ViewModel
{
    public class RoleManagementVM
    {
        public IdentityUser User { get; set; }
        public string? OldRole { get; set; }
        public int? DepartmentId { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    }
}
