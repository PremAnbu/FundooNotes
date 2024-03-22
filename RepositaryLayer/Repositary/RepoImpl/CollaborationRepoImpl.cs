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
                var query = @"
                            INSERT INTO Collaboration (UserId, NoteId, CollaboratorEmail) 
                            VALUES (@userId, @NoteId, @collaboratorEmail);
                            ";
                Collaboration coll = new Collaboration { NoteId = NoteId, UserId = user.Value, CollaboratorEmail = Request.Email };

                //if (!IsValidEmail(Request.Email))
                //{
                //    throw new InvalidEmailFormatException("Invalid email format");
                //}

                //  var emailExistsQuery = @"SELECT COUNT(*) FROM register WHERE Email = @collaboratorEmail";

                // var emailExistsParams = new { collaboratorEmail = Request.Email };

                using (var connection = _context.CreateConnection())
                {
                    // int emailCount = await connection.ExecuteScalarAsync<int>(emailExistsQuery, emailExistsParams);

                    //if (emailCount == 0)
                    //{
                    //    throw new NotFoundException($"Collaborator with email '{Request.Email}' Is Not A Registerd User please Register First and try Again.");
                    //}
                    // Check if table exists
                    //bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                    //    @"
                    //    SELECT COUNT(*)
                    //    FROM INFORMATION_SCHEMA.TABLES
                    //    WHERE TABLE_NAME = 'Collaboration';
                    //    "
                    //);

                    // Create table if it doesn't exist
                    //if (!tableExists)
                    //{
                    //    await connection.ExecuteAsync(
                    //        @"CREATE TABLE Collaboration (
                    //            CollaborationId INT IDENTITY(1, 1) PRIMARY KEY,
                    //            UserId INT,
                    //            NoteId INT,
                    //            CollaboratorEmail NVARCHAR(100),
                    //            CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId),
                    //            CONSTRAINT FK_NoteId FOREIGN KEY (NoteId) REFERENCES Notes (NoteId),
                    //            CONSTRAINT FK_CollaboratorEmail FOREIGN KEY (CollaboratorEmail) REFERENCES Users (Email)
                    //        );"
                    //    );
                    //}

                    // Insert collaborator
                    await connection.ExecuteAsync(query, coll);
                }
                _logger.LogInformation("Collaborator added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding collaborator.");
                throw; // Re-throw the exception to maintain the original behavior
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

    }

}
