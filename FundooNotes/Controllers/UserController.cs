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
        public ResponceStructure<string> createUser(UserRequest request)
        {
            try
            {
                var result = service.createUser(request);
                if (result == 1)
                {
                    try
                    {
                        // Produce Kafka message
                        var message = $"{request.Email} : User registered successfully";
                        var kafkaMessage = new Message<string, string>
                        {
                            Key = "user_created",
                            Value = message
                        };
                        _kafkaProducer.ProduceAsync(_configuration["Kafka:Topic"], kafkaMessage);
                        return new ResponceStructure<string>(true, "User registration successful");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return new ResponceStructure<string>(false, $"Error: {ex.Message}");
                    }
                }
                else
                {
                    return new ResponceStructure<string>(false, "Invalid Input");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex is InvalidUserInputException ? ex.Message : "An unexpected error occurred";
                _logger.LogError(ex.Message);

                return new ResponceStructure<string>(false, errorMessage);
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
        public ResponceStructure<string> Login(String Email, String password)
        {
            string token=null;
                try
                {
                UserResponce Responce =  service.Login(Email, password);
                if (Responce != null)
                {
                     token = GenerateToken(Responce);
                }
                _logger.LogInformation("Success");
                return new ResponceStructure<string>(200,"Login Sucessfull", token);
                }
                catch (Exception ex)
                {
                    if (ex is UserNotFoundException)
                    {
                    _logger.LogError(ex.Message);
                    return new ResponceStructure<string>(400,false, ex.Message);
                    }
                else 
                    {
                    _logger.LogError(ex.Message);
                    return new ResponceStructure<string>(400, false, ex.Message);
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

        [HttpPut("Email")]
        [UserExceptionHandlerFilter]
        public ResponceStructure<string> ChangePasswordRequest(String Email)
        {
            return new ResponceStructure<string>(200, service.ChangePasswordRequest(Email));
        }

        [HttpPut("NewPassword")]
        [UserExceptionHandlerFilter]
        public ResponceStructure<string> ChangePassword(String otp,String password)
        {
            return new ResponceStructure<string>(200, service.ChangePassword(otp,password));
        }
    }
}
