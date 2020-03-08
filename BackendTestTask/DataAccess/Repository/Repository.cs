using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BackendTestTask.DataAccess.Repository
{
    public class PostRepository : IRepository
    {
        private readonly BlogPostContext Context;
        private readonly IMemoryCache Cache;
        public PostRepository(BlogPostContext context, IMemoryCache _cache)
        {
            Context = context;
            Cache = _cache;
            if (!Context.Posts.Any())
            {
                SeedData.Initialize(Context);
            }
        }
        public async Task AddPostAsync(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException("post");
            }

            Post _post = new Post
            {
                Title = post.Title,
                Description = post.Description,
                Body = post.Body,
                Tags = post.Tags,
                Created = DateTime.Now,
                Comments = post.Comments
            };

            await Context.Posts.AddAsync(_post);
            Cache.Set(_post.Id, _post, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }
    

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await Context.Posts.Include(p => p.Comments).ToListAsync();
        }

        public async Task<Post> GetPostAsync(int id)
        {
            if (!Cache.TryGetValue(id, out Post post))
            {
                post = await Context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                {
                    Cache.Set(post.Id, post, 
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }

            return post;
           
        }
        public void RemovePost(Post post)
        {
            Context.Posts.Remove(post);
        }

        public void UpdatePost(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException("post");
            }

            Context.Posts.Update(post); 
        }
        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}

