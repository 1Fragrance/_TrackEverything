using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackEverything.EFStorage.Entities
{
    /// <summary>
    /// Entity that connect Task and Worker entities
    /// using Many to Many relationship
    /// </summary>
    public class EFTaskWorker
    {
        [Required] [Column("task_id")]
        public int TaskId { get; set; }

        [Required] [Column("worker_id")]
        public int WorkerId { get; set; }

        public EFWorker Worker { get; set; }
        public EFTask Task { get; set; }
    }
}