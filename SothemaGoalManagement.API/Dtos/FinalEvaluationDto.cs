using System;

namespace SothemaGoalManagement.API.Dtos
{
    public class FinalEvaluationDto
    {
        public int SheetId { get; set; }
        public int ValidatorId { get; set; }

        public string ValidatorComment { get; set; }

        public DateTime ValidatorValidationDateTime { get; set; }

        public string OwnerComment { get; set; }

        public DateTime OwnerValidationDateTime { get; set; }
    }
}