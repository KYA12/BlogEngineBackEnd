using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendTestTask.Models
{
    public static class SeedData
    {
        //Initialize local sql database with sample data
        public static void Initialize(BlogPostContext context)
        {

            Post post = new Post { Title = "Microsoft", Description = "Description 1", Body = "Body 1", Tags = "Tags 1", Created = DateTime.Now };
            context.Posts.Add(post);
            Comment fisrtComment = new Comment { UserName = "Tom", Message = "Message 1", Post = post, Created = DateTime.Now, EMail = "email1@test.com" };
            context.Comments.Add(fisrtComment);
            Comment secondComment = new Comment { UserName = "Jack", Message = "Message 2", Post = post, Created = DateTime.Now, EMail = "admin2@test.com" };
            context.Comments.Add(secondComment);
            Comment thirdComment = new Comment { UserName = "Mike", Message = "Message 3", Post = post, Created = DateTime.Now, EMail = "admin3@test.com" };
            context.Comments.Add(thirdComment);
            context.SaveChanges();

        }
    }
}
