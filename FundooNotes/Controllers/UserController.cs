using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.DTO.RequestDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundooNotes.Controllers
{
    [Authorize] // Add this attribute to UserController to enforce token-based authentication for all endpoints
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service ;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public UserController(IUserService service, IConfiguration configuration, IDistributedCache cache)
        {
            this.service = service;
            _configuration = configuration;
                _cache = cache;

        }

        [AllowAnonymous] // Allow this endpoint to be accessed without authentication
        [HttpPost]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> createUser(UserRequest request)
        {
            var result=await service.createUser(request);
            if(result==1)
                return Ok("User added successfully.");
            else
                return BadRequest("Failed to add User.");
        }

        [AllowAnonymous] // Allow this endpoint to be accessed without authentication
        [HttpGet("Login/{Email}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> Login(String Email, String password)
        {
            UserResponce Responce = await service.Login(Email, password);
            if (Responce != null)
            {
                var token = GenerateToken(Email);
                return Ok(token);
            }
            return Unauthorized();

        }
        private string GenerateToken(string email)
        {
            Console.WriteLine(_configuration["jwt:Key"]);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Minutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPut("forgotpass/{Email}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePasswordRequest(String Email)
        {
            return Ok( await service.ChangePasswordRequest(Email));
        }

        [HttpPut("otp/{otp}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePassword(String otp,String password)
        {
            return Ok(await service.ChangePassword(otp,password));
        }
    }
}
