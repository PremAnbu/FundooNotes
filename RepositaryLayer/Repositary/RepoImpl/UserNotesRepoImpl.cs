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
using System.Data;
using BuisinessLayer.Entity;

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
                Console.WriteLine(user.Value);

                using (var connection = context.CreateConnection())
                {
                    var parameters = new
                    {
                        request.Title,
                        request.Description,
                        request.Colour,
                        request.IsArchived,
                        request.IsPinned,
                        request.IsDeleted,
                        request.UserId
                    };

                   int rows= await connection.ExecuteAsync("spCreateUserNotes", parameters, commandType: CommandType.StoredProcedure);
                    if (rows>0)
                    {
                        _logger.LogInformation("User Notes Created Sucessfull");
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
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { NoteId = noteId };
                    var rowsAffected = await connection.ExecuteAsync("spDeleteNote", parameters, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting note with ID: {noteId}");
                throw;
            }
        }

        public async Task<IEnumerable<UserNotesResponce>> GetAllNoteByIdAsync(int userId)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { UserId = userId };
                    var notes = await connection.QueryAsync<UserNotesResponce>("spGetAllNotesByUserId", parameters, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching notes for user with ID: {userId}");
                throw;
            }
        }


        public async Task<IEnumerable<UserNotesResponce>> GetAllNoteAsync(string email)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { Email = email };
                    var notes = await connection.QueryAsync<UserNotesResponce>("spGetAllNotesByEmail", parameters, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching notes for user with email: {email}");
                throw;
            }
        }


        public async Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotes updatedNote)
        {
            try
            {
                int? userId = await GetUserIdByEmailAsync(updatedNote.Email);
                updatedNote.UserId = userId.Value;

                var selectQuery = "SELECT UserNotesId, Description, Title, Colour FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                using (var connection = context.CreateConnection())
                {
                    var currentNote = await connection.QueryFirstOrDefaultAsync<UserNotesResponce>(selectQuery, new { UserId = userId.Value, UserNotesId = noteId });
                    if (currentNote == null)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }

                    var parameters = new
                    {
                        UserId = userId.Value,
                        NoteId = noteId, // Corrected parameter name
                        Description = updatedNote.Description,
                        Title = updatedNote.Title,
                        Colour = updatedNote.Colour
                    };

                    await connection.ExecuteAsync("spUpdateNote", parameters, commandType: CommandType.StoredProcedure);

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

        private async Task<int> GetUserIdByEmailAsync(string email)
        {
            try
            {
                var parameters = new { Email = email };
                using (var connection = context.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<int>("spGetUserIdByEmail", parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching UserId for email: {email}");
                throw;
            }
        }


    }
}
