using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TrackEverything.Storage.Core.Entities;
using TrackEverything.Storage.Core.Infrastructure;
using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.ADOStorage.Repositories
{
    /// <summary>
    /// Repository with CRUD methods of project entity
    /// using ADO.NET
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private const string CreateCommand =
            " INSERT INTO Projects (name, shortname, description, creation_date, status) VALUES (@name, @shortname, @description, @creation_date, @status);  SET @current_task_id = SCOPE_IDENTITY();";

        private const string DeleteCommand =
            " BEGIN TRANSACTION; DELETE FROM Projects WHERE id = @id;  DELETE FROM Tasks  WHERE project_id = @id ;  COMMIT; ";

        private const string GetCommand = " SELECT * FROM Projects WHERE id = @id ";
        private const string GetAllCommand = " SELECT * FROM Projects ";

        private const string UpdateCommand =
            " UPDATE Projects SET name=@name, shortname=@shortname, description=@description, status=@status WHERE id=@id ";

        private const string GetTasksCommand = " SELECT * FROM Tasks WHERE project_id=@id ";

        private readonly string connectionPath;
        private readonly ILogger logger;

        public ProjectRepository(ILogger log)
        {
            connectionPath = new SQLDataAccess().ConnectionString;
            logger = log;
        }

        public int Create(Project item)
        {
            if (item != null)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(CreateCommand, connection);

                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = item.Name},
                            new SqlParameter("@shortname", SqlDbType.NVarChar) {Value = item.Shortname},
                            new SqlParameter("@description", SqlDbType.Text) {Value = item.Description},
                            new SqlParameter("@creation_date", SqlDbType.DateTime2) {Value = item.CreationDate},
                            new SqlParameter("@status", SqlDbType.Int) {Value = item.Status},
                            new SqlParameter("@current_task_id", SqlDbType.Int) {Direction = ParameterDirection.Output}
                        };
                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        var parent = Convert.ToInt32(command.Parameters["@current_task_id"].Value.ToString());
                        connection.Close();
                        logger.LogInformation($"DATABASE: Added {item.Name} project");
                        return parent;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in adding {item.Name} project");
                    throw;
                }

            return 0;
        }

        public void Delete(int id)
        {
            if (id != 0)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(DeleteCommand, connection);

                        var projectId = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(projectId);
                        command.ExecuteNonQuery();

                        connection.Close();
                        logger.LogInformation($"DATABASE: Deleted project #{id}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in deleting project #{id}");
                    throw;
                }
        }

        public Project Get(int id)
        {
            if (id != 0)
            {
                Project selectedProject = null;
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetCommand, connection);
                        var projectId = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(projectId);
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                            while (reader.Read())
                                selectedProject = new Project
                                {
                                    Id = (int) reader["id"],
                                    Name = (string) reader["name"],
                                    Shortname = (string) reader["shortname"],
                                    Description = (string) reader["description"],
                                    CreationDate = (DateTime) reader["creation_date"],
                                    Status = (int)reader["status"],
                                    Tasks = GetTasksByProjectId((int) reader["id"])
                                };
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading project #{id}");
                    throw;
                }

                return selectedProject;
            }

            return null;
        }

        public IEnumerable<Project> GetAll()
        {
            try
            {
                using (var connection = new SqlConnection(connectionPath))
                {
                    connection.Open();
                    var command = new SqlCommand(GetAllCommand, connection);
                    var reader = command.ExecuteReader();
                    var selectedProjects = new List<Project>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var tempProject = new Project
                            {
                                Id = (int) reader["id"],
                                Name = (string) reader["name"],
                                Shortname = (string) reader["shortname"],
                                Description = (string) reader["description"],
                                CreationDate = (DateTime) reader["creation_date"],
                                Status = (int)reader["status"],
                                Tasks = GetTasksByProjectId((int) reader["id"])
                            };
                            selectedProjects.Add(tempProject);
                        }
                    }
                    else
                    {
                        connection.Close();
                        return new List<Project>();
                    }

                    connection.Close();
                    logger.LogInformation("DATABASE: Uploaded all projects");
                    return selectedProjects;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("DATABASE: Error in uploading all projects");
                throw;
            }
        }

        public int Update(Project item, int id)
        {
            if (item != null && id != 0)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(UpdateCommand, connection);

                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@id", SqlDbType.Int) {Value = id},
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = item.Name},
                            new SqlParameter("@shortname", SqlDbType.NVarChar) {Value = item.Shortname},
                            new SqlParameter("@description", SqlDbType.Text) {Value = item.Description},
                            new SqlParameter("@status", SqlDbType.Int) {Value = item.Status}
                        };

                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        connection.Close();
                        logger.LogInformation($"DATABASE: Updated project #{id}");
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in updating project #{id}");
                    throw;
                }

            return 0;
        }

        public IEnumerable<Task> GetTasksByProjectId(int id)
        {
            if (id != 0)
            {
                var selectedTasks = new List<Task>();
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetTasksCommand, connection);
                        var param = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(param);
                        var reader = command.ExecuteReader();
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                var tempTask = new Task
                                {
                                    Id = (int) reader["id"],
                                    Name = (string) reader["name"],
                                    Estimation = (long) reader["time"],
                                    Status = (int)reader["status"],
                                    StartAt = (reader["started_at"] as DateTime?) ?? null,
                                    EndAt = (reader["ended_at"] as DateTime?) ?? null,
                                    CreationDate = (DateTime) reader["creation_date"],
                                    ProjectId = id,
                                    Executors = null
                                };
                                selectedTasks.Add(tempTask);
                            }
                        else
                        {
                            connection.Close();
                            return new List<Task>();
                        }
                        connection.Close();
                        return selectedTasks;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading project #{id} tasks");
                    throw;
                }  
            }
            return new List<Task>();
        }
    }
}