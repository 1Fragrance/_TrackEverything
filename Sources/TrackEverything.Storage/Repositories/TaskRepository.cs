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
    ///     Repository with CRUD methods of task entity
    ///     using ADO.NET
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private const string CreateCommand =
            " INSERT INTO Tasks (name, time, started_at, ended_at, creation_date, status, project_id) VALUES (@name, @time, @started_at, @ended_at, @creation_date, @status, @project_id);  SET @current_task_id = SCOPE_IDENTITY();";

        private const string DeleteCommand =
            " BEGIN TRANSACTION; DELETE FROM Tasks WHERE id = @id; DELETE FROM TaskWorkers  WHERE task_id = @id ;  COMMIT; ";

        private const string GetCommand = " SELECT * FROM Tasks WHERE id = @id";
        private const string GetAllCommand = " SELECT * FROM Tasks ";

        private const string UpdateCommand =
            " UPDATE Tasks SET name=@name, time=@time, started_at=@started_at, ended_at=@ended_at, status=@status, project_id=@project_id WHERE id=@id; DELETE FROM TaskWorkers WHERE task_id = @id ; SET @current_task_id = @id; ";

        private const string GetWorkersCommand =
            " SELECT * FROM Workers WHERE id IN (SELECT worker_id FROM TaskWorkers WHERE @id=task_id)";

        private const string GetProjectCommand = " SELECT * FROM Projects WHERE id = @id";

        private const string InsertTaskWorkerCommand =
            " INSERT INTO TaskWorkers(worker_id, task_id) VALUES (@worker_id, @task_id); ";

        private readonly string connectionPath;

        private readonly ILogger logger;

        public TaskRepository(ILogger log)
        {
            connectionPath = new SQLDataAccess().ConnectionString;
            logger = log;
        }

        public int Create(Task task)
        {
            if (task != null)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(CreateCommand, connection);

                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = task.Name},
                            new SqlParameter("@time", SqlDbType.BigInt) {Value = task.Estimation},
                            new SqlParameter("@status", SqlDbType.Int) {Value = task.Status},
                            new SqlParameter("@started_at", SqlDbType.DateTime2)
                                {Value = (object) task.StartAt ?? DBNull.Value},
                            new SqlParameter("@ended_at", SqlDbType.DateTime2)
                                {Value = (object) task.EndAt ?? DBNull.Value},
                            new SqlParameter("@creation_date", SqlDbType.DateTime2) {Value = task.CreationDate},
                            new SqlParameter("@project_id", SqlDbType.Int) {Value = task.ProjectId},
                            new SqlParameter("@current_task_id", SqlDbType.Int) {Direction = ParameterDirection.Output}
                        };

                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        var parent = Convert.ToInt32(command.Parameters["@current_task_id"].Value.ToString());
                        if (task.Executors != null)
                        {
                            var subcommand = new SqlCommand(InsertTaskWorkerCommand, connection);
                            foreach (var executor in task.Executors)
                            {
                                subcommand.Parameters.Add(new SqlParameter("@task_id", SqlDbType.Int) {Value = parent});
                                subcommand.Parameters.Add(new SqlParameter("@worker_id", SqlDbType.Int)
                                    {Value = executor.Id});
                                subcommand.ExecuteNonQuery();
                                subcommand.Parameters.Clear();
                            }
                        }

                        connection.Close();
                        logger.LogInformation($"DATABASE: Added {task.Name} task");
                        return parent;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in adding {task.Name} task");
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

                        var taskId = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(taskId);
                        command.ExecuteNonQuery();

                        connection.Close();
                        logger.LogInformation($"DATABASE: Deleted task #{id}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in deleting task #{id}");
                    throw;
                }
        }

        public Task Get(int id)
        {
            if (id != 0)
            {
                Task selectedTasks = null;
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetCommand, connection);
                        var TaskId = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(TaskId);
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            selectedTasks = new Task();
                            while (reader.Read())
                            {
                                selectedTasks.Id = (int) reader["id"];
                                selectedTasks.Name = (string) reader["name"];
                                selectedTasks.Estimation = (long) reader["time"];
                                selectedTasks.Status = (int) reader["status"];
                                selectedTasks.StartAt = reader["started_at"] as DateTime? ?? null;
                                selectedTasks.EndAt = reader["ended_at"] as DateTime? ?? null;
                                selectedTasks.CreationDate = (DateTime) reader["creation_date"];
                                selectedTasks.ProjectId = (int) reader["project_id"];
                                selectedTasks.Project = GetProjectByTaskId((int) reader["project_id"]);
                                selectedTasks.Executors = GetWorkersByTaskId((int) reader["id"]);
                            }
                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading task #{id}");
                    throw;
                }

                return selectedTasks;
            }

            return null;
        }

        public IEnumerable<Task> GetAll()
        {
            try
            {
                using (var connection = new SqlConnection(connectionPath))
                {
                    connection.Open();
                    var selectedTasks = new List<Task>();
                    var command = new SqlCommand(GetAllCommand, connection);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var tempTask = new Task
                            {
                                Id = (int) reader["id"],
                                Name = (string) reader["name"],
                                Estimation = (long) reader["time"],
                                Status = (int) reader["status"],
                                StartAt = reader["started_at"] as DateTime? ?? null,
                                EndAt = reader["ended_at"] as DateTime? ?? null,
                                CreationDate = (DateTime) reader["creation_date"],
                                ProjectId = (int) reader["project_id"],
                                Project = GetProjectByTaskId((int) reader["project_id"]),
                                Executors = GetWorkersByTaskId((int) reader["id"])
                            };
                            selectedTasks.Add(tempTask);
                        }
                    }
                    else
                    {
                        connection.Close();
                        return new List<Task>();
                    }

                    connection.Close();
                    logger.LogInformation("DATABASE: Uploaded all tasks");
                    return selectedTasks;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("DATABASE: Error in uploading all tasks");
                throw;
            }
        }

        public int Update(Task task, int id)
        {
            if (task != null && id != 0)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(UpdateCommand, connection);
                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@id", SqlDbType.Int) {Value = id},
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = task.Name},
                            new SqlParameter("@time", SqlDbType.BigInt) {Value = task.Estimation},
                            new SqlParameter("@status", SqlDbType.Int) {Value = task.Status},
                            new SqlParameter("@started_at", SqlDbType.DateTime2)
                                {Value = (object) task.StartAt ?? DBNull.Value},
                            new SqlParameter("@ended_at", SqlDbType.DateTime2)
                                {Value = (object) task.EndAt ?? DBNull.Value},
                            new SqlParameter("@project_id", SqlDbType.Int) {Value = task.ProjectId},
                            new SqlParameter("@current_task_id", SqlDbType.Int) {Direction = ParameterDirection.Output}
                        };
                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        if (task.Executors != null)
                        {
                            var subcommand = new SqlCommand(InsertTaskWorkerCommand, connection);
                            var parent = Convert.ToInt32(command.Parameters["@current_task_id"].Value.ToString());
                            foreach (var executor in task.Executors)
                            {
                                subcommand.Parameters.Add(new SqlParameter("@task_id", SqlDbType.Int) {Value = parent});
                                subcommand.Parameters.Add(new SqlParameter("@worker_id", SqlDbType.Int)
                                    {Value = executor.Id});
                                subcommand.ExecuteNonQuery();
                                subcommand.Parameters.Clear();
                            }
                        }

                        connection.Close();
                        logger.LogInformation($"DATABASE: Updated task #{id}");
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in updating task #{id}");
                    throw;
                }

            return 0;
        }

        public IEnumerable<Worker> GetWorkersByTaskId(int id)
        {
            if (id != 0)
            {
                var selectedWorkers = new List<Worker>();
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetWorkersCommand, connection);
                        var param = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(param);
                        var reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var tempWorker = new Worker
                                {
                                    Id = (int) reader["id"],
                                    Name = (string) reader["name"],
                                    Surname = (string) reader["surname"],
                                    MiddleName = (string) reader["middlename"],
                                    Position = (string) reader["position"],
                                    Tasks = null
                                };
                                selectedWorkers.Add(tempWorker);
                            }
                        }
                        else
                        {
                            connection.Close();
                            return new List<Worker>();
                        }

                        connection.Close();
                        return selectedWorkers;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading task #{id} executors");
                    throw;
                }
            }

            return new List<Worker>();
        }

        private Project GetProjectByTaskId(int id)
        {
            if (id != 0)
            {
                var selectedProject = new Project();
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetProjectCommand, connection);
                        var param = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(param);
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
                                    Status = (int) reader["status"]
                                };

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading task #{id} executors");
                    throw;
                }

                return selectedProject;
            }

            return null;
        }
    }
}