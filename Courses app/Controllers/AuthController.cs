using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Courses_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Courses_app.Controllers
{

    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if(login != null && !login.Email.IsNullOrEmpty() && !login.Password.IsNullOrEmpty()) {
                try
                {
                    var res = await _userService.GetByEmailAndPassword(login.Email, login.Password);
                    if (res != null)
                    {
                        var token = GenerateJwtToken(res);
                        return Ok(new { token });
                    }

                    return Unauthorized("There is no user with given credetials");

                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An unexpected error occured");
                }
            }
            else
            {
                return BadRequest("Invalid username or password");
            }

        }

        [HttpPost("basic-user-registration")]
        public async Task<IActionResult> BasicUserRegistration([FromBody] BasicUserRegistrationModel registration)
        {
            if (registration != null && !registration.Email.IsNullOrEmpty() && !registration.Password.IsNullOrEmpty() && !registration.Username.IsNullOrEmpty()
                && !registration.Name.IsNullOrEmpty() && !registration.Surname.IsNullOrEmpty())
            {

                try
                {
                    BasicUser basicUser = new BasicUser();
                    basicUser.Email = registration.Email;
                    basicUser.Password = registration.Password;
                    basicUser.Username = registration.Username;
                    basicUser.Role = UserRole.User;
                    basicUser.Name = registration.Name;
                    basicUser.Surname = registration.Surname;
                    basicUser.IsActive = true;


                    var res = await _userService.AddBasicUser(basicUser);
                    if (res != null)
                    {
                        var token = GenerateJwtToken(res);
                        return Ok(new { token });
                    }
                    else
                    {
                        return StatusCode(500, "An error occurred while creating the user.");
                    }

                }
                catch(EmailAlreadyExistsException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (UsernameAlreadyExistsException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An unexpected error occurred.");
                }
            }

            return BadRequest("Invalid username,email or password");
        }

        [HttpPost("author-registration")]
        public async Task<IActionResult> AuthorRegistration([FromBody] AuthorRegistrationModel registration)
        {
            if (registration != null && !registration.Email.IsNullOrEmpty() && !registration.Password.IsNullOrEmpty() && !registration.Username.IsNullOrEmpty()
                && !registration.Name.IsNullOrEmpty() && !registration.Surname.IsNullOrEmpty() && !registration.PayPalEmail.IsNullOrEmpty())
            {

                try
                {
                    Author author = new Author();
                    author.Name = registration.Name;
                    author.Surname = registration.Surname;
                    author.Username = registration.Username;
                    author.Email = registration.Email;
                    author.Password = registration.Password;
                    author.PayPalEmail = registration.PayPalEmail;
                    author.Role = UserRole.Author;
                    author.IsActive = true;


                    var res = await _userService.AddAuthor(author);
                    if (res != null)
                    {
                        var token = GenerateJwtToken(res);
                        return Ok(new { token });
                    }
                    else
                    {
                        return StatusCode(500, "An error occurred while creating the user.");
                    }

                }
                catch (EmailAlreadyExistsException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (UsernameAlreadyExistsException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An unexpected error occurred.");
                }
            }

            return BadRequest("Invalid username,email or password");
        }

        [Authorize(Policy ="UserOnly")]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok("This is a protected endpoint");
        }


        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role.ToString()),
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
