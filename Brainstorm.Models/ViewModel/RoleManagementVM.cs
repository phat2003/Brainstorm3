using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Models.ViewModel
{
    public class RoleManagementVM
    {
        public IdentityUser User { get; set; }
        public string OldRole { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
