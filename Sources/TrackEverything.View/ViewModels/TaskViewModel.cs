using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrackEverything.View.Properties;
using TrackEverything.View.ValidationAttributes;

namespace TrackEverything.View.ViewModels
{
    /// <summary>
    /// View Model for task entity
    /// which working with HTML page through API methods
    /// </summary>
    public class TaskViewModel
    {
        public TaskViewModel()
        {
            Executors = new List<WorkerViewModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; } = Convert.ToInt32(DefaultValues.ResourceManager.GetObject("TaskStatus"));

        [Required(ErrorMessage = "Estimation is required")]
        public string Estimation { get; set; }

        [DateLessThan("StartAt", ErrorMessage = "Start date cannot be more than end date")]
        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        [Required(ErrorMessage = "Can't take current date")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.Date;

        public int ProjectId { get; set; }
        public ProjectViewModel Project { get; set; }

        public List<WorkerViewModel> Executors { get; set; }
    }
}