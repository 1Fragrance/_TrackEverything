using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackEverything.EFStorage.Entities
{
    /// <summary>
    /// Project entity class for working with Entity Framework
    /// </summary>
    public class EFProject
    {
        public EFProject()
        {
            Tasks = new List<EFTask>();
        }

        [Key] [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("shortname")]
        public string Shortname { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }

        public virtual IList<EFTask> Tasks { get; set; }
    }
}