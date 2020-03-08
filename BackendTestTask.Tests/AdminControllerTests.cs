using System;
using System.Collections.Generic;
using System.Text;
using BackendTestTask.DataAccess.Repository;
using BackendTestTask.Models;
using BackendTestTask.Controllers;
using Moq;
using AutoMapper;
using Xunit;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BackendTestTask.DTO;
using BackendTestTask.Automapper;
using System.Threading.Tasks;
using Serilog;

namespace BackendTestTask.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<IRepository> mockRepository;//Create Moq object of Repository
        private readonly AdminController adminController;//Create Moq object of AdminController
        private readonly List<Post> expectedPosts;//Create list of Posts
        private readonly IMapper mapper;// create object of AutoMapper 
        private readonly Mock<ILogger> logger;//create Moq object of Serilog

        public AdminControllerTests()
        {
            mockRepository = new Mock<IRepository>();

            /*Create and setup AutoMapper profile*/
            AutoMapperProfile profile = new AutoMapperProfile();
           
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profile);
            });

            mapper = config.CreateMapper();
            logger = new Mock<ILogger>();

            //Setup moq controller with moq objects and local version of automapper
            adminController = new AdminController(mockRepository.Object, mapper, logger.Object);
            expectedPosts = GetExpectedPost();

            //Setup all methods of the moq object of Repository  
            mockRepository.Setup(m => m.GetAllPostsAsync()).Returns(Task.FromResult(expectedPosts));
          
            mockRepository.Setup(m => m.AddPostAsync(It.IsAny<Post>())).Returns(
                (Post target) =>
                {
                    expectedPosts.Add(target);
                    return Task.FromResult(true);
                });

            mockRepository.Setup(m => m.GetPostAsync(It.IsAny<int>())).Returns(
              (int Id) =>
              {
                  var post = expectedPosts.FirstOrDefault(p => p.Id == Id);
                  return Task.FromResult(post);
              });

            mockRepository.Setup(m => m.UpdatePost(It.IsAny<Post>())).Callback(
               (Post target) =>
               {
                   var post = expectedPosts.FirstOrDefault(p => p.Id == target.Id);
                   post = target;
               });

            mockRepository.Setup(m => m.RemovePost(It.IsAny<Post>())).Callback(
            (Post post) =>
            {
                expectedPosts.Remove(post);
            });
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = adminController.GetAllPosts();
       
            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllPosts()
        {
            // Act
            var okResult = adminController.GetAllPosts().Result as OkObjectResult;
            List<PostDTO> listPostDTO;
            listPostDTO = mapper.Map<List<PostDTO>>(expectedPosts);
            
            // Assert
            var posts = Assert.IsType<List<PostDTO>>(okResult.Value);
          
            Assert.Equal(listPostDTO.Count, (okResult.Value as List<PostDTO>).Count);
        }

        private static List<Post> GetExpectedPost()
        {
            List<Post> posts = new List<Post>();
            Post post = new Post
            {
                Title = "Microsoft",
                Description = "Hello",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<Comment>(),
                Id = 1
            };
            List<Comment> comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    UserName = "Hey",
                    Message = "Hey people",
                    EMail = "admin@test.com",
                    PostId = 1,
                    Post = post,
                    Created = DateTime.Now
                }
            };
            post.Comments = comments;
            posts.Add(post);
            return posts;
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Act
            var notFoundResult = adminController.GetPostById(5);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsOkResult()
        {
            // Arrange
            int testId = 1;
            
            // Act
            var okResult = adminController.GetPostById(testId);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsRightPost()
        {
            // Arrange
            int testId = 1;

            // Act
            var okResult = adminController.GetPostById(testId).Result as OkObjectResult;

            // Assert
            Assert.IsType<PostDTO>(okResult.Value);
            Assert.Equal(testId, (okResult.Value as PostDTO).Id);
        }

        [Fact]
        public void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            PostDTO descriptionMissingItem = new PostDTO
            {
                Title = "Microsoft",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<CommentDTO>(),
            };
            List<CommentDTO> comments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    Id = 1,
                    UserName = "Hey",
                    Message = "Hey people",
                    EMail = "admin@test.com",
                    PostId = 1,
                    Post = descriptionMissingItem,
                    Created = DateTime.Now
                }
            };
            descriptionMissingItem.Comments = comments;
            adminController.ModelState.AddModelError("Description", "Required");

            // Act
            var badResponse = adminController.AddPost(descriptionMissingItem);
          
            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse.Result);
        }

        [Fact]
        public void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            PostDTO post = new PostDTO
            {
                Title = "Microsoft",
                Description = "Hello",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<CommentDTO>(),
                Id = 1
            };
            List<CommentDTO> comments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    Id = 1,
                    UserName = "Hey",
                    Message = "Hey people",
                    EMail = "admin@test.com",
                    PostId = 1,
                    Post = post,
                    Created = DateTime.Now
                }
            };
            post.Comments = comments;

            // Act
            var createdResponse = adminController.AddPost(post);

            // Assert
            Assert.IsType<OkResult>(createdResponse.Result);
        }
        
        [Fact]
        public void Update_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            PostDTO descriptionMissingItem = new PostDTO
            {
                Title = "Microsoft",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<CommentDTO>(),
                Id = 1
            };
            List<CommentDTO> comments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    Id = 1,
                    UserName = "Hey",
                    Message = "Hey people",
                    EMail = "admin@test.com",
                    PostId = 1,
                    Post = descriptionMissingItem,
                    Created = DateTime.Now
                }
            };
            descriptionMissingItem.Comments = comments;
            adminController.ModelState.AddModelError("Description", "Required");

            // Act
            var badResponse = adminController.AddPost(descriptionMissingItem);

            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse.Result);
        }

        [Fact]
        public void Update_ValidObjectPassed_ReturnsUpdateResponse()
        {
            // Arrange
            PostDTO post = new PostDTO
            {
                Title = "Microsoft",
                Description = "People",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<CommentDTO>(),
                Id = 1
            };

            List<CommentDTO> comments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    Id = 1,
                    UserName = "Hey",
                    Message = "Hey people",
                    EMail = "admin@test.com",
                    PostId = 1,
                    Post = post,
                    Created = DateTime.Now
                }
            };
            post.Comments = comments;
            int Id = 1;

            // Act
            var updatedResponse = adminController.UpdatePost(post, Id);

            // Assert
            Assert.IsType<OkResult>(updatedResponse.Result);
        }

        [Fact]
        public void Update_NotExistingPost_ReturnsNotFoundResponse()
        {
            // Arrange
            int Id = 5;
            PostDTO post = new PostDTO
            {
                Title = "Microsoft",
                Description = "People",
                Body = "Hey",
                Tags = "",
                Created = DateTime.Now,
                Comments = new List<CommentDTO>(),
                Id = 2
            };

            List<CommentDTO> comments = new List<CommentDTO>();
            comments.Add(new CommentDTO
            {
                Id = 1,
                UserName = "Hey",
                Message = "Hey people",
                EMail = "admin@test.com",
                PostId = 2,
                Post = post,
                Created = DateTime.Now
            });
            post.Comments = comments;

            // Act
            var notFoundResponse = adminController.UpdatePost(post, Id);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResponse.Result);
        }

        [Fact]
        public void Remove_NotExistingIdPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingId = 5;

            // Act
            var notFoundResponse = adminController.DeletePost(notExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResponse.Result);
        }

        [Fact]
        public void Remove_ExistingIdPassed_ReturnsOkResult()
        {
            // Arrange
            var existingId = 1;

            // Act
            var okResponse = adminController.DeletePost(existingId);

            // Assert
            Assert.IsType<OkResult>(okResponse.Result);
        }
    }
}
