using Microsoft.Extensions.Logging;
using BuisinessLayer.Entity;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System.Data;
using Dapper;
using RepositaryLayer.GlobalCustomException;


namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserImplementationRL : IUserRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<UserImplementationRL> _logger;
        public UserImplementationRL(DapperContext context, ILogger<UserImplementationRL> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public int createUser(UserEntity entity)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserFirstName", entity.UserFirstName);parameters.Add("@UserLastName", entity.UserLastName);
                parameters.Add("@UserEmail", entity.UserEmail);parameters.Add("@UserPassword", entity.UserPassword);
                var connection = context.CreateConnection();
                return  connection.Execute("spCreateUser", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating user: {ex.Message}");
                throw;
            }
        }

        public UserEntity GetUserByEmail(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);
                Console.WriteLine(email);
                using (IDbConnection connection = context.CreateConnection())
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    var user = connection.QueryFirstOrDefault<UserEntity>("spGetUserByEmail", parameters,commandType: CommandType.StoredProcedure);
                    Console.WriteLine(user.UserEmail);
                    return user;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching user by email: {email}");
                throw new UserNotesNotPresentException("User Not Found By Email"); // Re-throw the exception to maintain the error propagation
            }
        }

        public int UpdatePassword(string mailid, string password)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserEmail", mailid);
                parameters.Add("@UserPassword", password);
                IDbConnection connection = context.CreateConnection();
                return  connection.Execute("spUpdatePassword", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating password for user with email: {mailid}");
                throw new UserNotesNotPresentException("User Not Found By Email"); // Re-throw the exception to maintain the error propagation
            }
        }


    }
}