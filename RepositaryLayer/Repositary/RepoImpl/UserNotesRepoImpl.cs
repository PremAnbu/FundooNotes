using CommonLayer.Models.ResponceDto;
using Microsoft.Data.SqlClient;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Extensions.Logging;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserNotesRepoImpl : IUserNotesRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<UserNotesRepoImpl> _logger;

        public UserNotesRepoImpl(DapperContext context, ILogger<UserNotesRepoImpl> logger)
        {
            context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotes request)
        {
            try
            {
                int? user = await GetUserIdByEmailAsync(request.Email);
                request.UserId = user.Value;
                var insertQuery = @"INSERT INTO UserNotes (Title,Description, Colour, IsArchived,IsPinned, IsDeleted, UserId)
                                    VALUES (@Title, @Description, @Colour, @IsArchived, @IsPinned ,@IsDeleted, @UserId)";
                using (var connection = context.CreateConnection())
                {
                    bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>(
                                    @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserNotes';");

                    var note = await connection.ExecuteAsync(insertQuery, request);

                    if (tableExists)
                    {
                        return await GetAllNoteByIdAsync(request.UserId);
                    }
                    else
                    {
                        // Return an empty list or handle the case when the table doesn't exist
                        return new List<UserNotesResponce>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user notes.");
                throw; // Re-throw the exception to maintain the original behavior
            }
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var deleteQuery = "DELETE FROM UserNotes WHERE UserNotesId = @NoteId";
            using (var connection = context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(deleteQuery, new { NoteId = noteId});
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<UserNotesResponce>> GetAllNoteByIdAsync(int userId)
        {
            var selectQuery = "SELECT * FROM UserNotes WHERE UserId = @UserId";
            using (var connection = context.CreateConnection())
            {
                var notes = await connection.QueryAsync<UserNotesResponce>(selectQuery, new { UserId = userId });
                return notes.Reverse().ToList();
            }
        }

        public async Task<IEnumerable<UserNotesResponce>> GetAllNoteAsync(string email)
        {
            int? user = await GetUserIdByEmailAsync(email);
            int userId = user.Value;
            var selectQuery = "SELECT * FROM UserNotes WHERE UserId = @UserId";
            using (var connection = context.CreateConnection())
            {
                var notes = await connection.QueryAsync<UserNotesResponce>(selectQuery, new { UserId = userId });
                return notes.Reverse().ToList();
            }
        }

        public async Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotes updatedNote)
        {
            int? user = await GetUserIdByEmailAsync(updatedNote.Email);
            updatedNote.UserId = user.Value;

            var selectQuery = "SELECT UserNotesId, Description, Title, Colour FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @NoteId";

            var updateQuery = @"UPDATE UserNotes SET Description = @Description, 
            Title = @Title, 
            Colour = @Colour 
            WHERE UserId = @UserId AND UserNotesId = @NoteId";
            string prevTitle, prevDescription, prevColour;
            try
            {
                using (var connection = context.CreateConnection())
                {
                    // Retrieve the current note details from the database
                    var currentnote = await connection.QueryFirstOrDefaultAsync<UserNotesResponce>(selectQuery, new { UserId = user.Value, NoteId = noteId });


                    if (currentnote == null)
                    {
                        throw new FileNotFoundException("Note not found");
                    }

                    // Store the previous values of the note
                    prevTitle = currentnote.Title;
                    prevDescription = currentnote.Description;
                    prevColour = currentnote.Colour;

                    // Execute the update query with the provided parameters
                    await connection.ExecuteAsync(updateQuery, new
                    {
                        Description = CheckInput(updatedNote.Description, prevDescription),
                        Title = CheckInput(updatedNote.Title, prevTitle),
                        Colour = CheckInput(updatedNote.Colour, prevColour),
                        UserId = user.Value,
                        NoteId = noteId
                    });

                    // Retrieve the updated note
                    var updatedNoteResponse = await connection.QueryFirstOrDefaultAsync<UserNotesResponce>(selectQuery, new { UserId = user.Value, NoteId = noteId });

                    return updatedNoteResponse;
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception("An error occurred in the repository layer", ex);
            }
        }


        private object CheckInput(string description, string prevDescription)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<UserNotesResponce>> IUserNotesRepo.CreateUserNotes(UserNotes request)
        {
            throw new NotImplementedException();
        }

        private async Task<int?> GetUserIdByEmailAsync(string email)
        {
            var selectQuery = $"SELECT UserId FROM Register WHERE UserEmail = @Email";

            using (var connection = context.CreateConnection())
            {
                // Use parameterized query to prevent SQL injection
                var userId = await connection.QueryFirstOrDefaultAsync<int?>(selectQuery, new { Email = email });
                return userId;
            }
        }

    }
}
