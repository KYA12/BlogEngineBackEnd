using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.Models;
using BackendTestTask.DataAccess.Repository;
using AutoMapper;
using BackendTestTask.DTO;
using Serilog;

namespace BackendTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IRepository Repository;
        private readonly IMapper Mapper;
        private readonly ILogger _logger;
        public PostsController(IRepository repository, IMapper _mapper, ILogger logger)
        {
            Repository = repository;
            Mapper = _mapper;
            _logger = logger;
        }

        // GET
        // api/admin
        /// <summary>
        /// Retrieves Posts list
        /// </summary>
        /// <returns>A response with posts list</returns>
        /// <response code="200">Returns the posts list</response>
        /// <response code="404">If post is not exists</response>
        /// <response code="500">If there was an internal server error</response>
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PostDTO>>> GetAllPosts()
        {
            try
            {
                var posts = Mapper.Map<List<PostDTO>>(await Repository.GetAllPostsAsync());
                _logger.Information($"Returned all posts from database.");
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetAllPosts action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET
        // api/admin/1
        /// <summary>
        /// Retrieves a Post by ID
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns>A response with post</returns>
        /// <response code="200">Returns post list</response>
        /// <response code="404">If post is not exists</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetPostById(int id)
        {
            try
            {
                var post = Mapper.Map<PostDTO>(await Repository.GetPostAsync(id));

                if (post == null)
                {
                    _logger.Error($"Post with id: {id}, hasn't been found in database.");
                    return NotFound();
                }

                _logger.Information($"Returned post with id: {id}");
                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error inside GetPostById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT
        // api/admin/1
        /// <summary>
        /// Updates an existing Post with new Comment
        /// </summary>
        /// <param name="comment">Comment model</param>
        /// <returns>A response as update post result</returns>
        /// <response code="200">If post was updated successfully</response>
        /// <response code="400">For bad request</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> AddComment([FromBody] CommentDTO comment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Error("Invalid CommentDTO object sent from the client");
                    return BadRequest(ModelState);
                }

                Comment _comment = Mapper.Map<Comment>(comment);
                Post post = await Repository.GetPostAsync(comment.PostId);
                post.Comments.Add(_comment);
                Repository.UpdatePost(post);
                await Repository.SaveAsync();
                _logger.Information("Comment added to the existing Post in the database");
                return Ok();
            }  
            catch (Exception ex)
            {
                _logger.Error($"Error in AddComment: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
