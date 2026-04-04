using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Models
{
    public class React // Thumbs Up/Down [cite: 22]
    {
        [Key]
        public int Id { get; set; }

        // 1: Like, -1: Dislike, 0: None
        public int ReactValue { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        public int IdeaId { get; set; }
        [ForeignKey("IdeaId")]
        [ValidateNever]
        public Idea Idea { get; set; }
    }
}
