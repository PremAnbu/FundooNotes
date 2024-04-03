using Azure.Core;
using CommonLayer.Models.RequestDto;
using Dapper;
using Microsoft.Extensions.Logging;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class CollaborationRepoImpl : ICollaborationRepo
    {
        private readonly DapperContext _context;
        private readonly ILogger<CollaborationRepoImpl> _logger;

        public CollaborationRepoImpl(DapperContext context, ILogger<CollaborationRepoImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddCollaborator(int NoteId, CollaborationRequest Request)
        {
            try
            {
                int? user = await GetUserIdByEmailAsync(Request.Email);
                var query = @"INSERT INTO Collaboration (UserId, UserNotesId, CollaboratorEmail) 
                    VALUES (@UserId, @UserNotesId, @CollaboratorEmail)";
                var checkQuery = @"SELECT COUNT(*) FROM Collaboration WHERE UserId = @UserId AND UserNotesId = @UserNotesId";
                Collaboration coll = new Collaboration { UserNotesId = NoteId, UserId = user.Value, CollaboratorEmail = Request.Email };

                using (var connection = _context.CreateConnection())
                {
                    int existingCollaborations = await connection.ExecuteScalarAsync<int>(checkQuery, new { UserId = user.Value, UserNotesId = NoteId });
                    if (existingCollaborations == 0)
                    {
                        await connection.ExecuteAsync(query, coll);
                        _logger.LogInformation("Collaborator added successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Collaboration already exists for the specified UserId and UserNotesId.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding collaborator.");
                throw;
            }
        }

        private async Task<int?> GetUserIdByEmailAsync(string email)
        {
            var selectQuery = $"SELECT UserId FROM Register WHERE UserEmail = @Email";

            using (var connection = _context.CreateConnection())
            {
                // Use parameterized query to prevent SQL injection
                var userId = await connection.QueryFirstOrDefaultAsync<int?>(selectQuery, new { Email = email });
                return userId;
            }
        }

        //private bool IsValidEmail(string email)
        //{
        //    string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        //    return Regex.IsMatch(email, pattern);
        //}

        public async Task<IEnumerable<Collaboration>> GetCollaborators(int CollaborationId)
        {
            var query = $"SELECT * FROM Collaboration where CollaborationId={CollaborationId}";
            using (var connection = _context.CreateConnection())
            {
                var collaborators = await connection.QueryAsync<Collaboration>(query);
                return collaborators;
            }

        }

        public async Task<bool> RemoveCollaborator(int CollaborationId)
        {
            var query = $"DELETE FROM Collaboration WHERE CollaborationId = {CollaborationId}";
              using (var connection = _context.CreateConnection())
                {
                int rowsAffected = await connection.ExecuteAsync(query);
                return rowsAffected > 0; 
        }

        }
    }
}

