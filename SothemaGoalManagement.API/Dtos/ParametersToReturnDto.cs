using System;

namespace SothemaGoalManagement.API.Dtos
{
    public class ParametersToReturnDto
    {
        public int Id { get; set; }

        public string Event { get; set; }

        public DateTime StartEvent { get; set; }

        public DateTime EndEvent { get; set; }

        public int EvaluationFilelId { get; set; }

        public bool ToggleChangeAxisWeight { get; set; }

    }
}