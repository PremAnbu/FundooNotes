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

        public bool AddCollaborator(int NoteId, CollaborationRequest Request, int userId)
        {
            try
            {
                var query = @"INSERT INTO Collaboration (UserId, UserNotesId, CollaboratorEmail) 
                    VALUES (@UserId, @UserNotesId, @CollaboratorEmail)";
                var checkQuery = @"SELECT COUNT(*) FROM Collaboration WHERE UserId = @UserId AND UserNotesId = @UserNotesId";
                Collaboration coll = new Collaboration { UserNotesId = NoteId, UserId = userId, CollaboratorEmail = Request.Email };

                using (var connection = _context.CreateConnection())
                {
                    int existingCollaborations =  connection.ExecuteScalar<int>(checkQuery, new { UserId = userId, UserNotesId = NoteId });
                    if (existingCollaborations == 0)
                    {
                         connection.Execute(query, coll);
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

        //private int? GetUserIdByEmailAsync(string email)
        //{
        //    var selectQuery = $"SELECT UserId FROM Users WHERE UserEmail = @Email";

        //    using (var connection = _context.CreateConnection())
        //    {
        //        // Use parameterized query to prevent SQL injection
        //        var userId =  connection.QueryFirstOrDefault<int?>(selectQuery, new { Email = email });
        //        return userId;
        //    }
        // }

        //private bool IsValidEmail(string email)
        //{
        //    string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        //    return Regex.IsMatch(email, pattern);
        //}

        public Collaboration GetCollaborators(int collaborationId, int userId)
        {
            var query = $"SELECT * FROM Collaboration WHERE CollaborationId = {collaborationId} AND UserId = {userId}";
            using (var connection = _context.CreateConnection())
            {
                return connection.QueryFirstOrDefault<Collaboration>(query);
            }
        }


        public bool RemoveCollaborator(int CollaborationId, int userId)
        {
            var query = $"DELETE FROM Collaboration WHERE CollaborationId = {CollaborationId} and UserId={userId}";
              using (var connection = _context.CreateConnection())
                {
                int rowsAffected =  connection.Execute(query);
                return rowsAffected > 0; 
                }
        }
    }
}

