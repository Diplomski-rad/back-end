using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);


                var roleClaim = HttpContext.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim == null)
                {
                    return Unauthorized("Role not found");
                }

                string role = roleClaim.Value;

                if (Enum.TryParse(role, out UserRole userRole))
                {

                    var user = await _userService.UpdateUser(userId, request, userRole);

                    if (user == null)
                    {
                        return Unauthorized();
                    }

                    return Ok("User data successfully changed.");
                }

                return Unauthorized("Role not found");

            }
            catch (UsernameAlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }

        [Authorize(Policy = "AuthorOnly")]
        [HttpGet("author")]
        public async Task<IActionResult> GetAuthor()
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var author = await _userService.GetAuthor(userId);
                if (author == null)
                {
                    return NotFound("User not found");
                }

                return Ok(author);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured.");
            }
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet("basic-user")]
        public async Task<IActionResult> GetBasicUser()
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Unauthorized("User not found");
                }
                var userId = long.Parse(userIdClaim.Value);

                var user = await _userService.GetBasicUser(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(users);

            }catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchUserModel model)
        {
            try
            {
                if (Enum.TryParse(model.Flag, out SearchUserFlag flag) && model.Query != null && model.Query != string.Empty)
                {
                    var users = await _userService.Search(model.Query, flag);
                    return Ok(users);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{userId}/ban")]
        public async Task<IActionResult> BanUser(long userId)
        {
            try
            {
                var banedUserId = await _userService.BanUser(userId);
                return Ok(banedUserId);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{userId}/unban")]
        public async Task<IActionResult> UnbanUser(long userId)
        {
            try
            {
                var unbanedUserId = await _userService.UnbanUser(userId);
                return Ok(unbanedUserId);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }


        //[HttpGet("admin")]
        //public async Task<IActionResult> CreateAdmin()
        //{
        //    try
        //    {
        //        User user = new User() { Email = "admin@example.com", Username = "admin", Role = UserRole.Admin, Password = "password123", IsActive = true };
        //        var adminId = await _userService.AddAdmin(user);

        //        return Ok(adminId);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An unexpected error occured");
        //    }
        //}
    }
}
