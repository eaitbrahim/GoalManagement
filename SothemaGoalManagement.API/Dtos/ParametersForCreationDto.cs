using System;
using System.ComponentModel.DataAnnotations;

namespace SothemaGoalManagement.API.Dtos
{
    public class ParametersForCreationDto
    {
        [Required] public int EvaluationFileId { get; set; }
        [Required] public string Event { get; set; }
        public DateTime StartEvent { get; set; }
        public DateTime EndEvent { get; set; }
        public bool ToggleChangeAxisWeight { get; set; }
    }
}