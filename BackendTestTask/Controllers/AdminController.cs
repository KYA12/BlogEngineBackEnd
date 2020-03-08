using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.Models;
using BackendTestTask.DataAccess.Repository;
using BackendTestTask.DTO;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using BackendTestTask.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BackendTestTask.Controllers
{
    // Add Authorize policy with random generate ApiKey
    [Authorize(Policy = Policies.OnlyAdmin)]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {  
        private readonly IRepository Repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        //Get services using Dependency Injection
        public AdminController(IRepository repository, IMapper mapper, ILogger logger)
        {
            Repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // POST
        // api/admin/
        /// <summary>
        /// Creates a new Post
        /// </summary>
        /// <param name="post">Post model</param>
        /// <returns>A response with new post</returns>
        /// <response code="200">Returns the post list</response>
        /// <response code="201">A response as creation of post</response>
        /// <response code="400">For bad request</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> AddPost([FromBody] PostDTO post)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Error("Invalid post object sent from client.");
                    return BadRequest(ModelState);
                }

                Post _post = _mapper.Map<Post>(post);
                await Repository.AddPostAsync(_post);
                await Repository.SaveAsync();
                _logger.Information("Post was successfully added to the db");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in AddPosts action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }         
        }

        // DELETE
        // api/admin/1
        /// <summary>
        /// Deletes an existing Post
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>A response as delete post result</returns>
        /// <response code="200">If post was deleted successfully</response>
        /// <response code="404">If post is not exists</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                var post = await Repository.GetPostAsync(id);
                if (post == null)
                {
                    _logger.Error($"Post with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                Repository.RemovePost(post);
                await Repository.SaveAsync();
                _logger.Information($"Post with {id} was deleted in database");
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.Error($"Error in DeletePost action:{ex.Message}");
                return StatusCode(500, "Internal server error");
            }
            
        }

        // GET
        // api/admin
        /// <summary>
        /// Retrieves Posts list
        /// </summary>
        /// <returns>A response with posts list</returns>
        /// <response code="200">Returns the posts list</response>
        /// <response code="500">If there was an internal server error</response>
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetAllPosts()
        {
            try
            {
                var posts = _mapper.Map<List<PostDTO>>(await Repository.GetAllPostsAsync());
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
                var post = _mapper.Map<PostDTO>(await Repository.GetPostAsync(id));

                if (post == null)
                {
                    _logger.Error($"Post with id: {id}, hasn't been found in database.");
                    return NotFound();
                }

                _logger.Information($"Returned post with id: {id}");
                return Ok(post);
            }
            catch(Exception ex)
            {
                _logger.Error($"Error inside GetPostById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT
        // api/admin/1
        /// <summary>
        /// Updates an existing Post
        /// </summary>
        /// <param name="Id">Post ID</param>
        /// <param name="post">Post model</param>
        /// <returns>A response as update post result</returns>
        /// <response code="200">If post was updated successfully</response>
        /// <response code="400">For bad request</response>
        /// <response code="404">If post is not exists</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdatePost([FromBody] PostDTO post, int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Post _post = await Repository.GetPostAsync(Id);

                    if (_post == null)
                    {
                        _logger.Error("PostDTO object sent from client is null.");
                        return NotFound();
                    }
                   
                    _post = _mapper.Map<Post>(post);
                    Repository.UpdatePost(_post);
                    await Repository.SaveAsync();
                    _logger.Information($"Post with id: {Id} was successfully updated in database.");
                    return Ok();

                }
                _logger.Error($"Invalid post object sent from client");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in UpdatePost action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }    
    }
}
