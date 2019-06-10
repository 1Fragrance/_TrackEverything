using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackEverything.View.ViewModels
{
    /// <summary>
    /// View Model for worker entity
    /// which working with HTML page through API methods
    /// </summary>
    public class WorkerViewModel
    {
        public WorkerViewModel()
        {
            Tasks = new List<TaskViewModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string Surname { get; set; }

        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string Position { get; set; }

        public List<TaskViewModel> Tasks { get; set; }
    }
}