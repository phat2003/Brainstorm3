using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Chủ Đề")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Hạn nộp ý tưởng")]
        public DateTime ClosureDate { get; set; } // Hạn chót để đăng Idea mới

        [Required]
        [Display(Name = "Hạn đóng bình luận")]
        public DateTime FinalClosureDate { get; set; } // Hạn chót để đăng Comment
    }
}
