using System;
using System.ComponentModel.DataAnnotations;

namespace SothemaGoalManagement.API.Dtos
{
    public class ParametersForCreationDto
    {
        [Required] public int EvaluationFileId { get; set; }
        [Required] public string Event { get; set; }
        [Required] public DateTime StartEvent { get; set; }
        [Required] public DateTime EndEvent { get; set; }
    }
}