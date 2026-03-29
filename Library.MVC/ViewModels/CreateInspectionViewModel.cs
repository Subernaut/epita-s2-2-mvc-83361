using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library.Domain;

namespace Library.MVC.ViewModels
{
    public class CreateInspectionViewModel
    {
        [Required]
        [Display(Name = "Premises")]
        public int PremisesId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Inspection Date")]
        public DateTime InspectionDate { get; set; } = DateTime.Today;

        [Required]
        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public Outcome Outcome { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public List<SelectListItem> PremisesList { get; set; } = new List<SelectListItem>();
    }
}