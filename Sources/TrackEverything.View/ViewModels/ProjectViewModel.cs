using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrackEverything.View.Properties;

namespace TrackEverything.View.ViewModels
{
    /// <summary>
    /// View Model for project entity
    /// which working with HTML page through API methods
    /// </summary>
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            Tasks = new List<TaskViewModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "20 is the Max Length")]
        [MinLength(3, ErrorMessage = "3 is the Min Length")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Shortname is required")]
        [MaxLength(10, ErrorMessage = "10 is the Max Length")]
        [MinLength(2, ErrorMessage = "2 is the Min Length")]
        public string Shortname { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Can't take current date")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.Date;

        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; } = Convert.ToInt32(DefaultValues.ResourceManager.GetObject("ProjectStatus"));

        public List<TaskViewModel> Tasks { get; set; }
    }
}