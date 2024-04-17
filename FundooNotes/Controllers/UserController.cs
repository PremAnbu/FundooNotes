using BuisinessLayer.CustomException;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using BusinessLayer.CustomException;
using CommonLayer.Models;
using Confluent.Kafka;
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
   // [Authorize] // Add this attribute to UserController to enforce token-based authentication for all endpoints
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly IProducer<string, string> _kafkaProducer;
        private readonly IConsumer<string, string> _kafkaConsumer;
        private readonly ILogger<UserController> _logger;


        public UserController(IUserService service, IConfiguration configuration, IDistributedCache cache , IProducer<string,string> kafkaProducer, IConsumer<string,string> kafkaConsumer, ILogger<UserController> logger)
        {
            this.service = service;
            _configuration = configuration;
            _cache = cache;
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to Book Controller");

            // Start Kafka consumer background task
            Task.Run(() => ConsumeKafkaMessages(new CancellationTokenSource().Token));
        }

     //   [AllowAnonymous] // Allow this endpoint to be accessed without authentication
        [HttpPost]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> createUser(UserRequest request)
        {
                try
                {
                   var result = await service.createUser(request);
                   if (result == 1)
                   {
                    try
                    {
                        // Produce Kafka message
                        var message = $"{request.Email} : User register Successfully";
                        var kafkaMessage = new Message<string, string>
                        {
                            Key = "user_created",
                            Value = message
                        };
                        await _kafkaProducer.ProduceAsync(_configuration["Kafka:Topic"], kafkaMessage);

                        //return Ok(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
                    }
                    var response = new ResponceStructure<UserResponce>
                        {
                            Success = true,
                            Message = "User Registration Successful"
                        };
                        return Ok(response);
                    }
                    else
                        return BadRequest("invalid input");
                }
                catch (Exception ex)
                {
                    if (ex is InvalidUserInputException)
                    {
                       var response = new ResponceStructure<UserResponce>
                       {
                        Success = false,
                        Message = ex.Message
                       };
                    _logger.LogError(ex.Message);

                    return BadRequest(response);
                    }
                    else 
                    {
                       var response = new ResponceStructure<UserResponce>
                       {
                        Success = false,
                        Message = ex.Message
                       };
                    _logger.LogError(ex.Message);

                    return BadRequest(response);
                    }
                }
        }
        private async Task ConsumeKafkaMessages(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _kafkaConsumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    Console.WriteLine($"Received Kafka message: {message}");
                }
            }
            catch (OperationCanceledException ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
            }
            finally
            {
                _kafkaConsumer.Close();
            }
        }


      //  [AllowAnonymous] // Allow this endpoint to be accessed without authentication
        [HttpGet("{Email}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> Login(String Email, String password)
        {
            string token=null;
                try
                {
                UserResponce Responce = await service.Login(Email, password);
                if (Responce != null)
                {
                     token = GenerateToken(Responce);
                }
                var response = new ResponceStructure<string>
                    {
                        Message = "Login Sucessfull",
                        Data = token
                    };
                _logger.LogInformation("Success");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    if (ex is UserNotFoundException)
                    {
                        var response = new ResponceStructure<string>
                        {
                            Success = false,
                            Message = ex.Message
                        };
                    _logger.LogError(ex.Message);

                    return Conflict(response);
                    }
                    else 
                    {
                       var response = new ResponceStructure<string>
                       {
                        Success = false,
                        Message = ex.Message
                       };
                    _logger.LogError(ex.Message);

                    return BadRequest(response);
                    }
                }
        }

        private string GenerateToken(UserResponce user)
        {
            Console.WriteLine(_configuration["jwt:Key"]);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.FirstName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Minutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPut("{Email}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePasswordRequest(String Email)
        {
            return Ok( await service.ChangePasswordRequest(Email));
        }

        [HttpPut("{otp}/{new_password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePassword(String otp,String password)
        {
            return Ok(await service.ChangePassword(otp,password));
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _kafkaProducer?.Dispose();
        //        _kafkaConsumer?.Dispose();
        //        _cancellationTokenSource?.Dispose();
        //    }
        //}
    }
}
