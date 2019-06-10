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
    /// Repository with CRUD methods of worker entity
    /// using ADO.NET
    /// </summary>
    public class WorkerRepository : IWorkerRepository
    {
        private const string CreateCommand =
            " INSERT INTO Workers (name, surname, middlename, position) VALUES (@name, @surname, @middlename, @position); SET @current_worker_id = SCOPE_IDENTITY(); ";

        private const string DeleteCommand =
            " BEGIN TRANSACTION;  DELETE FROM Workers WHERE id = @id;  DELETE FROM TaskWorkers  WHERE worker_id = @id ; COMMIT; ";

        private const string GetCommand = " SELECT * FROM Workers WHERE id = @id; ";
        private const string GetAllCommand = " SELECT * FROM Workers; ";

        private const string UpdateCommand =
            " UPDATE Workers  SET name=@name, surname=@surname, middlename=@middlename, position=@position WHERE id = @id; DELETE FROM TaskWorkers WHERE worker_id = @id ; SET @current_worker_id = @id; ";

        private const string GetTasksCommand =
            " SELECT * FROM Tasks WHERE id IN (SELECT task_id FROM TaskWorkers WHERE @id=worker_id)";

        private const string InsertTaskWorkerCommand =
            " INSERT INTO TaskWorkers(worker_id, task_id) VALUES (@worker_id, @task_id); ";

        private readonly string connectionPath;

        private readonly ILogger logger;

        public WorkerRepository(ILogger log)
        {
            connectionPath = new SQLDataAccess().ConnectionString;
            logger = log;
        }

        public int Create(Worker worker)
        {
            if (worker != null)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(CreateCommand, connection);
                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = worker.Name},
                            new SqlParameter("@surname", SqlDbType.NVarChar) {Value = worker.Surname},
                            new SqlParameter("@middlename", SqlDbType.NVarChar) {Value = worker.MiddleName},
                            new SqlParameter("@position", SqlDbType.NVarChar) {Value = worker.Position},
                            new SqlParameter("@current_worker_id", SqlDbType.Int)
                                {Direction = ParameterDirection.Output}
                        };
                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        var parent = Convert.ToInt32(command.Parameters["@current_worker_id"].Value.ToString());
                        if (worker.Tasks != null)
                        {
                            var subcommand = new SqlCommand(InsertTaskWorkerCommand, connection);
                            foreach (var task in worker.Tasks)
                            {
                                subcommand.Parameters.Add(new SqlParameter("@task_id", SqlDbType.Int)
                                    {Value = task.Id});
                                subcommand.Parameters.Add(
                                    new SqlParameter("@worker_id", SqlDbType.Int) {Value = parent});
                                subcommand.ExecuteNonQuery();
                                subcommand.Parameters.Clear();
                            }
                        }

                        connection.Close();
                        logger.LogInformation($"DATABASE: Added {worker.Name} worker");
                        return parent;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in adding {worker.Name} worker");
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
                        var workerId = new SqlParameter("@id", SqlDbType.Int) {Value = id};

                        command.Parameters.Add(workerId);
                        command.ExecuteNonQuery();

                        connection.Close();
                        logger.LogInformation($"DATABASE: Deleted worker #{id}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in deleting worker #{id}");
                    throw;
                }
        }

        public Worker Get(int id)
        {
            if (id != 0)
            {
                Worker selectedWorker = null;
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(GetCommand, connection);
                        var WorkerId = new SqlParameter("@id", SqlDbType.Int) {Value = id};
                        command.Parameters.Add(WorkerId);
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            selectedWorker = new Worker();
                            while (reader.Read())
                            {
                                selectedWorker.Id = (int) reader["id"];
                                selectedWorker.Name = (string) reader["name"];
                                selectedWorker.Surname = (string) reader["surname"];
                                selectedWorker.MiddleName = (string) reader["middlename"];
                                selectedWorker.Position = (string) reader["position"];
                                selectedWorker.Tasks = GetTasksByWorkerId((int) reader["id"]);
                            }
                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in uploading worker #{id}");
                    throw;
                }

                return selectedWorker;
            }

            return null;
        }

        public IEnumerable<Worker> GetAll()
        {
            try
            {
                using (var connection = new SqlConnection(connectionPath))
                {
                    connection.Open();
                    var command = new SqlCommand(GetAllCommand, connection);
                    var reader = command.ExecuteReader();
                    var selectedWorkers = new List<Worker>();

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
                                Tasks = GetTasksByWorkerId((int) reader["id"])
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
                    logger.LogInformation("DATABASE: Uploaded all workers");
                    return selectedWorkers;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("DATABASE: Error in uploading all workers");
                throw;
            }
        }

        public int Update(Worker worker, int id)
        {
            if (worker != null && id != 0)
                try
                {
                    using (var connection = new SqlConnection(connectionPath))
                    {
                        connection.Open();
                        var command = new SqlCommand(UpdateCommand, connection);

                        var prm = new List<SqlParameter>
                        {
                            new SqlParameter("@id", SqlDbType.Int) {Value = id},
                            new SqlParameter("@name", SqlDbType.NVarChar) {Value = worker.Name},
                            new SqlParameter("@surname", SqlDbType.NVarChar) {Value = worker.Surname},
                            new SqlParameter("@middlename", SqlDbType.NVarChar) {Value = worker.MiddleName},
                            new SqlParameter("@position", SqlDbType.NVarChar) {Value = worker.Position},
                            new SqlParameter("@current_worker_id", SqlDbType.Int)
                                {Direction = ParameterDirection.Output}
                        };
                        command.Parameters.AddRange(prm.ToArray());
                        command.ExecuteNonQuery();

                        if (worker.Tasks != null)
                        {
                            var subcommand = new SqlCommand(InsertTaskWorkerCommand, connection);
                            var parent = Convert.ToInt32(command.Parameters["@current_worker_id"].Value.ToString());
                            foreach (var task in worker.Tasks)
                            {
                                subcommand.Parameters.Add(new SqlParameter("@task_id", SqlDbType.Int)
                                    {Value = task.Id});
                                subcommand.Parameters.Add(
                                    new SqlParameter("@worker_id", SqlDbType.Int) {Value = parent});
                                subcommand.ExecuteNonQuery();
                                subcommand.Parameters.Clear();
                            }
                        }

                        connection.Close();
                        logger.LogInformation($"DATABASE: Updated worker #{id}");
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"DATABASE: Error in updating worker #{id}");
                    throw;
                }

            return 0;
        }

        public IEnumerable<Task> GetTasksByWorkerId(int id)
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
                                    StartAt = (reader["started_at"] as DateTime?) ?? null,
                                    EndAt = (reader["ended_at"] as DateTime?) ?? null,
                                    CreationDate = (DateTime) reader["creation_date"],
                                    Status = (int)reader["status"],
                                    ProjectId = (int) reader["project_id"],
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
                    logger.LogCritical($"DATABASE: Error in uploading worker's #{id} tasks");
                    throw;
                } 
            }
            return new List<Task>();
        }
    }
}