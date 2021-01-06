using System;
using System.Collections.Generic;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Dtos
{
    public class EvaluationSheetToReturnDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public int Year { get; set; }

        public string PoleName { get; set; }
        public string GoalsStatus { get; set; }
        public DateTime ValidatorValidationDateTime { get; set; }
    }

}