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

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class NotesLabelRepoImpl : INotesLabelRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<NotesLabelRepoImpl> _logger;
        public NotesLabelRepoImpl(DapperContext context, ILogger<NotesLabelRepoImpl> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public async Task<int> CreateNewLabel(string labelName)
        {
            try
            {
                if (await IsLabelNameExists(labelName))
                {
                    _logger.LogError("Label name already exists.");
                    throw new LabelNotPresentException ("Label name already exists.");
                }

                var insertQuery = "INSERT INTO Labels (LabelName) VALUES (@LabelName)";

                using (var connection = context.CreateConnection())
                {
                    return await connection.ExecuteAsync(insertQuery, new { LabelName = labelName });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new label.");
                throw;
            }
        }


        public async Task<int> DeleteLabel(string labelName)
        {
            try
            {
                if (!await IsLabelNameExists(labelName))
                {
                    // Log and throw custom exception if label name does not exist
                    _logger.LogError("Label name does not exist.");
                    throw new LabelNotPresentException("Label name does not exist.");
                }

                var deleteQuery = "DELETE FROM Labels WHERE LabelName = @LabelName";
                using (var connection = context.CreateConnection())
                {
                    return await connection.ExecuteAsync(deleteQuery, new { LabelName = labelName });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the label.");
                throw;
            }
        }

        public async Task<int> UpdateLabelName(string labelName, string newLabelName)
        {
            try
            {
                if (!await IsLabelNameExists(labelName))
                {
                    _logger.LogError("Label name does not exist.");
                    throw new LabelNotPresentException("Label name does not exist.");
                }
                var updateQuery = "UPDATE Labels SET LabelName = @NewLabelName WHERE LabelName = @LabelName";
                using (var connection = context.CreateConnection())
                {
                    return await connection.ExecuteAsync(updateQuery, new { NewLabelName = newLabelName, LabelName = labelName });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the label name.");
                throw;
            }
        }


        public async Task<bool> IsLabelNameExists(string labelName)
        {
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var query = "SELECT COUNT(*) FROM Labels WHERE LabelName = @LabelName";
                    var count = await connection.QueryFirstOrDefaultAsync<int>(query, new { LabelName = labelName });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking label name existence.");
                throw;
            }
        }

    }
}
