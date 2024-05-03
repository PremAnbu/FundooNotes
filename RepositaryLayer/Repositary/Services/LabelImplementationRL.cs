using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositaryLayer.GlobalCustomException;
using RepositaryLayer.Entity;
using CommonLayer.Models.ResponceDto;
using System.Data;
using ModelLayer.Models.ResponceDto;
using System.Collections;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class LabelImplementationRL : ILabelRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<LabelImplementationRL> _logger;
        public LabelImplementationRL(DapperContext context, ILogger<LabelImplementationRL> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public int CreateNewLabel(string labelName, int noteId, int userId)
        {
            try
            {
                if ( IsLabelNameExists(labelName))
                {
                    _logger.LogError("Label name already exists.");
                    throw new LabelNotPresentException ("Label name already exists.");
                }

                var insertQuery = "INSERT INTO Labels (LabelName,UserId,UserNotesId) VALUES (@LabelName,@UserId,@UserNotesId)";

                using (var connection = context.CreateConnection())
                {
                    return connection.Execute(insertQuery, new { LabelName = labelName , UserId = userId , UserNotesId =noteId});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new label.");
                throw;
            }
        }


        public int DeleteLabel(string labelName, int noteId, int userId)
        {
            try
            {
                if (! IsLabelNameExists(labelName))
                {
                    // Log and throw custom exception if label name does not exist
                    _logger.LogError("Label name does not exist.");
                    throw new LabelNotPresentException("Label name does not exist.");
                }

                var deleteQuery = "DELETE FROM Labels WHERE LabelName = @LabelName and UserId = @UserId and UserNotesId=@UserNotesId";
                using (var connection = context.CreateConnection())
                {
                    return connection.Execute(deleteQuery, new { LabelName = labelName, UserId = userId, UserNotesId = noteId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the label.");
                throw;
            }
        }

        public int UpdateLabelName(string labelName, string newLabelName, int noteId, int userId)
        {
            try
            {
                if (! IsLabelNameExists(labelName))
                {
                    _logger.LogError("Label name does not exist.");
                    throw new LabelNotPresentException("Label name does not exist.");
                }
                var updateQuery = "UPDATE Labels SET LabelName = @NewLabelName WHERE LabelName = @LabelName,UserId=@UserId";
                using (var connection = context.CreateConnection())
                {
                    return  connection.Execute(updateQuery, new { NewLabelName = newLabelName, LabelName = labelName , UserId = userId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the label name.");
                throw;
            }
        }


        public bool IsLabelNameExists(string labelName)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var query = "SELECT COUNT(*) FROM Labels WHERE LabelName = @LabelName";
                    var count =  connection.QueryFirstOrDefault<int>(query, new { LabelName = labelName });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking label name existence.");
                throw;
            }
        }

        public List<LabelResponce> GetLabel(int userId)
        {
            try
            {
                string query = "SELECT * FROM Labels WHERE UserId = @UserId";

                using (var connection = context.CreateConnection())
                {
                    var parameters = new { UserId = userId };
                    var labels =  connection.Query<LabelResponce>(query, parameters);
                    return labels.Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching labels for user with ID: {userId}");
                throw;
            }
        }




    }
}
