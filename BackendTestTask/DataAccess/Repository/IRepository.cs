using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.Models;

namespace BackendTestTask.DataAccess.Repository
{
    public interface IRepository
    {
        Task<Post> GetPostAsync(int id);
        Task<List<Post>> GetAllPostsAsync();
        void RemovePost(Post post);
        Task AddPostAsync(Post post);
        void UpdatePost(Post post);
        Task SaveAsync();
    }
}
