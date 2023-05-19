using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uoW;
         public LikesController(IUnitOfWork UoW)
         {
            _uoW = UoW;
   
         }

         [HttpPost("{username}")]
         public async Task<ActionResult> AddLike(string username) 
         {
            var sourceUserId = User.GetUserId();
            var likedUser = await _uoW.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uoW.LikesRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");
         
            var userLike = await _uoW.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _uoW.Complete()) return Ok();

            return BadRequest("Failed to like user");
         }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _uoW.LikesRepository.GetUserLikes(likesParams);
            
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize
            , users.TotalCount, users.TotalPages));

            return Ok(users);
        }


    }
}