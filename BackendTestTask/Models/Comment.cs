using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendTestTask.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
        public DateTime? Created { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public Post Post { get; set; }
        [Required(ErrorMessage = "EMail is required"), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string EMail { get; set; }
    }
}
