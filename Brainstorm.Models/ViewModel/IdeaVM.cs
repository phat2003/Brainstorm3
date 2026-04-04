using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Models.ViewModel
{
    public class IdeaVM
    {
        public Idea idea { get; set; }
        [ValidateNever]
        public View view { get; set; }
        [ValidateNever]
        public React react { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> TopicList { get; set; }
        [ValidateNever]
        public int LikeCount { get; set; }
        [ValidateNever]
        public int DislikeCount { get; set; }
        [ValidateNever]
        public int ViewCount { get; set; }
        // Thêm dòng này vào trong class IdeaVM
        [ValidateNever]
        public IEnumerable<Comment> Comments { get; set; }
    }
}
