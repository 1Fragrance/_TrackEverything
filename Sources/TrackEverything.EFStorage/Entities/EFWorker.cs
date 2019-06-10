using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrackEverything.EFStorage.Entities
{
    /// <summary>
    /// Worker entity class for working with Entity Framework
    /// </summary>
    public class EFWorker
    {
        [Key, Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("surname")]
        public string Surname { get; set; }
        [Column("middlename")]
        public string MiddleName { get; set; }
        [Column("position")]
        public string Position { get; set; }

        public virtual IList<EFTaskWorker> TaskWorkers { get; set; }
        public EFWorker()
        {
            TaskWorkers = new List<EFTaskWorker>();
        }
    }
}
