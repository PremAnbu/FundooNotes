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
           this. context = context;
           _logger = logger;
        }

        public async Task<int> createUser(UserEntity entity)
        {
            try
            {
                String query = "insert into Register values (@UserFirstName,@UserLastName,@UserEmail,@UserPassword)";
                var connection = context.CreateConnection();
                return await connection.ExecuteAsync(query, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user.");
                throw;
            }
        }
        public async Task<UserEntity> GetUserByEmail(string email)
        {
            try
            {
                String Query = "Select * from Register where UserEmail = @Email";
                IDbConnection connection = context.CreateConnection();
                return await connection.QueryFirstAsync<UserEntity>(Query, new { Email = email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching user by email: {email}");
                throw;
            }
        }
        public async Task<int> UpdatePassword(string mailid, string password)
        {
            try
            {
                String Query = "update Register set UserPassword = @Password where UserEmail = @mail";
                IDbConnection connection = context.CreateConnection();
                return await connection.ExecuteAsync(Query, new { mail = mailid, Password = password });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating password for user with email: {mailid}");
                throw;
            }
        }
    }
}
