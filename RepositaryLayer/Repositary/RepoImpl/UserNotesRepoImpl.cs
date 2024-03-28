﻿using CommonLayer.Models.ResponceDto;
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
            this.context = context;
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
                throw; 
            }
        }
        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var deleteQuery = "update UserNotes set IsDeleted=1 WHERE UserNotesId = @NoteId";
            using (var connection = context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(deleteQuery, new { NoteId = noteId});
                return rowsAffected > 0;
            }
        }
        public async Task<IEnumerable<UserNotesResponce>> GetAllNoteByIdAsync(int userId)
        {
            var selectQuery = "SELECT * FROM UserNotes WHERE UserId = @UserId and IsDeleted=0";
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
            var selectQuery = "SELECT * FROM UserNotes WHERE UserId = @UserId and IsDeleted=0";
            using (var connection = context.CreateConnection())
            {
                Console.WriteLine("Ms sql Server database");
                var notes = await connection.QueryAsync<UserNotesResponce>(selectQuery, new { UserId = userId });
                return notes.Reverse().ToList();
            }
        }

        public async Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotes updatedNote)
        {
            try
            {
                int? userId = await GetUserIdByEmailAsync(updatedNote.Email);
                updatedNote.UserId = userId.Value;

                var selectQuery = "SELECT UserNotesId, Description, Title, Colour FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                var updateQuery = "UPDATE UserNotes SET Description = @Description, Title = @Title, Colour = @Colour WHERE UserId = @UserId AND UserNotesId = @UserNotesId";
                _logger.LogInformation("Updating note...");

                using (var connection = context.CreateConnection())
                {
                    var currentNote = await connection.QueryFirstOrDefaultAsync<UserNotesResponce>(selectQuery, new { UserId = userId.Value, UserNotesId = noteId });
                    if (currentNote == null)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }

                    await connection.ExecuteAsync(updateQuery, new { Description = updatedNote.Description,
                        Title = updatedNote.Title, Colour = updatedNote.Colour,
                        UserId = updatedNote.UserId, UserNotesId = noteId });

                    // Retrieve the updated note
                    var updatedNoteResponse = await connection.QueryFirstOrDefaultAsync<UserNotesResponce>(selectQuery, new { UserId = updatedNote.UserId, UserNotesId = noteId });
                    _logger.LogInformation("Note updated successfully.");
                    return updatedNoteResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the repository layer");
                throw;
            }
        }



        private async Task<int?> GetUserIdByEmailAsync(string email)
        {
            var selectQuery = $"SELECT UserId FROM Register WHERE UserEmail = @Email";

            using (var connection = context.CreateConnection())
            {
                var userId = await connection.QueryFirstOrDefaultAsync<int?>(selectQuery, new { Email = email });
                return userId;
            }
        }

    }
}
