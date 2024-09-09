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

                    var author = await _userService.UpdateUser(userId, request, userRole);

                    if (author == null)
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
    }
}
