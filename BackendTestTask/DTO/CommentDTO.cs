using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendTestTask.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
        public DateTime? Created { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public PostDTO Post { get; set; }
        [Required(ErrorMessage = "Email is required"), RegularExpression(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$")]
        public string EMail { get; set; }
    }
}
