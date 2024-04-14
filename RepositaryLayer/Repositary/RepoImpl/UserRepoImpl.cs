using Microsoft.Extensions.Logging;
using BuisinessLayer.Entity;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System.Data;
using Dapper;


namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserRepoImpl : IUserRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<UserRepoImpl> _logger;
        public UserRepoImpl(DapperContext context, ILogger<UserRepoImpl> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public async Task<int> createUser(UserEntity entity)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserFirstName", entity.UserFirstName);parameters.Add("@UserLastName", entity.UserLastName);
                parameters.Add("@UserEmail", entity.UserEmail);parameters.Add("@UserPassword", entity.UserPassword);
                var connection = context.CreateConnection();
                return await connection.ExecuteAsync("spCreateUser", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating user: {ex.Message}");
                throw;
            }
        }

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);

                using (IDbConnection connection = context.CreateConnection())
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    var user = await connection.QueryFirstOrDefaultAsync<UserEntity>("spGetUserByEmail",parameters,commandType: CommandType.StoredProcedure);
                    Console.WriteLine(user.UserEmail);
                    return user;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching user by email: {email}");
                throw; // Re-throw the exception to maintain the error propagation
            }
        }

        public async Task<int> UpdatePassword(string mailid, string password)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserEmail", mailid);
                parameters.Add("@UserPassword", password);
                IDbConnection connection = context.CreateConnection();
                return await connection.ExecuteAsync("spUpdatePassword", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating password for user with email: {mailid}");
                throw;
            }
        }


    }
}