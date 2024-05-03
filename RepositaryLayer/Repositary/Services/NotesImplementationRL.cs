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
using RepositaryLayer.GlobalCustomException;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class NotesImplementationRL : INotesRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<NotesImplementationRL> _logger;
        public NotesImplementationRL(DapperContext context, ILogger<NotesImplementationRL> logger)
        {
            this.context = context;
            _logger = logger;
        }
        public List<NotesResponce> CreateUserNotes(Notes request)
        {
            try
            {
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

                   int rows=  connection.Execute("spCreateUserNotes", parameters, commandType: CommandType.StoredProcedure);
                    if (rows>0)
                    {
                        _logger.LogInformation("User Notes Created Sucessfull");
                        return  GetAllNoteById(request.UserId);
                    }
                    else
                    {
                        return new List<NotesResponce>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user notes.");
                throw new UserNotesNotPresentException("Error Occurred while creating user notes"); 
            }
        }
        public bool DeleteNote(int noteId,int userId)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { NoteId = noteId,UserId=userId };
                    var rowsAffected =  connection.Execute("spDeleteNote", parameters, commandType: CommandType.StoredProcedure);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting note with ID: {noteId}");
                throw new UserNotesNotPresentException("Error Occurred while finding notes");
            }
        }

        public List<NotesResponce> GetAllNoteById(int userId)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { UserId = userId };
                    var notes =  connection.Query<NotesResponce>("spGetAllNotesByUserId", parameters, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching notes for user with ID: {userId}");
                throw;
            }
        }

        public List<NotesResponce> GetNoteById(int noteId, int userId)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { UserNotesId = noteId,UserId=userId };
                    var notes =  connection.Query<NotesResponce>("spGetNoteByNoteId", parameters, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching notes for user with ID: {noteId}");
                throw;
            }
        }


        public List<NotesResponce> GetAllNotes(int userId)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var parameters = new { UserId = userId };
                    var notes =  connection.Query<NotesResponce>("spGetAllNotesByUserId", parameters, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching notes for user with email");
                throw;
            }
        }


        public NotesResponce UpdateNote(int noteId, Notes updatedNote)
        {
            try
            {
                var selectQuery = "SELECT UserNotesId, Description, Title, Colour FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                using (var connection = context.CreateConnection())
                {
                    var currentNote =  connection.QueryFirstOrDefault<NotesResponce>(selectQuery, new { UserId = updatedNote.UserId, UserNotesId = noteId });
                    if (currentNote == null)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }

                    var parameters = new
                    {
                        UserId = updatedNote.UserId,
                        NoteId = noteId, // Corrected parameter name
                        Description = updatedNote.Description,
                        Title = updatedNote.Title,
                        Colour = updatedNote.Colour
                    };

                     connection.Execute("spUpdateNote", parameters, commandType: CommandType.StoredProcedure);

                    // Retrieve the updated note
                    var updatedNoteResponse =  connection.QueryFirstOrDefault<NotesResponce>(selectQuery, new { UserId = updatedNote.UserId, UserNotesId = noteId });
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

        public int UpdateNoteColour(int noteId, int userId, string colour)
        {
            try
            {
                var selectQuery = "SELECT UserNotesId FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                using (var connection = context.CreateConnection())
                {
                    var currentNoteId = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });
                    if (currentNoteId == 0)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }

                    var updateQuery = "UPDATE UserNotes SET Colour = @Colour WHERE UserNotesId = @UserNotesId AND UserId = @UserId";
                    var parameters = new { UserId = userId, UserNotesId = noteId, Colour = colour };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the repository layer");
                throw;
            }
        }

        public int UpdateArchive(int noteId, int userId)
        {
            try
            {
                var selectQuery = "SELECT UserNotesId FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                using (var connection = context.CreateConnection())
                {
                    var currentNoteId = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });
                    if (currentNoteId == 0)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }
                    var updateQuery = "UPDATE UserNotes SET IsArchived = CASE WHEN IsArchived  = 0 THEN 1 ELSE 0 END WHERE UserNotesId = @UserNotesId AND UserId = @UserId";
                    var parameters = new { UserId = userId, UserNotesId = noteId };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the repository layer");
                throw;
            }
        }

        public int UpdateTrash(int noteId, int userId)
        {
            try
            {
                var selectQuery = "SELECT UserNotesId FROM UserNotes WHERE UserId = @UserId AND UserNotesId = @UserNotesId";

                using (var connection = context.CreateConnection())
                {
                    var currentNote = connection.QueryFirstOrDefault<int>(selectQuery, new { UserId = userId, UserNotesId = noteId });

                    if (currentNote == null)
                    {
                        _logger.LogError("Note not found");
                        throw new FileNotFoundException("Note not found");
                    }

                    var updateQuery = "UPDATE UserNotes SET IsDeleted = CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END WHERE UserNotesId = @UserNotesId AND UserId = @UserId";

                    var parameters = new { UserId = userId, UserNotesId = noteId };

                    return connection.Execute(updateQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the repository layer");
                throw;
            }
        }




        //private async Task<int> GetUserIdByEmailAsync(string email)
        //{
        //    try
        //    {
        //        var parameters = new { Email = email };
        //        using (var connection = context.CreateConnection())
        //        {
        //            var result = await connection.QueryFirstOrDefaultAsync<int>("spGetUserIdByEmail", parameters, commandType: CommandType.StoredProcedure);
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error occurred while fetching UserId for email: {email}");
        //        throw;
        //    }
        //}
    }
}
