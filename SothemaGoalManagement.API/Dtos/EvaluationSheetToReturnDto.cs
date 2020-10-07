using System;

namespace SothemaGoalManagement.API.Dtos
{
    public class EvaluationSheetToReturnDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int Year { get; set; }

        public string Email { get; set; }
        public string PoleName { get; set; }
        public string GoalsStatus { get; set; }
        public DateTime ValidatorValidationDateTime { get; set; }

    }
}