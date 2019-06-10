using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackEverything.EFStorage.Entities
{
    /// <summary>
    /// Task entity class for working with Entity Framework
    /// </summary>
    public class EFTask
    {
        public EFTask()
        {
            TaskWorkers = new List<EFTaskWorker>();
        }

        [Key] [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("time")]
        public long Estimation { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }

        [Column("started_at")]
        public DateTime? StartAt { get; set; }

        [Column("ended_at")]
        public DateTime? EndAt { get; set; }

        [Column("project_id")]
        public int? ProjectId { get; set; }

        public virtual EFProject Project { get; set; }
        public virtual IList<EFTaskWorker> TaskWorkers { get; set; }
    }
}