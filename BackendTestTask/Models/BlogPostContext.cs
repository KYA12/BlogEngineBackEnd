using Microsoft.EntityFrameworkCore;

namespace BackendTestTask.Models
{
    public class BlogPostContext:DbContext
    {
        public BlogPostContext(DbContextOptions<BlogPostContext> options)
             : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
      
    }
}
