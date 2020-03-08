using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendTestTask.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Tags is required")]
        public string Tags { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public List<CommentDTO> Comments { get; set; }
    }
}
