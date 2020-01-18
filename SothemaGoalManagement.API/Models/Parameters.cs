using System;

namespace SothemaGoalManagement.API.Models
{
    public class Parameters
    {
        public int Id { get; set; }

        public string Event { get; set; }

        public DateTime StartEvent { get; set; }

        public DateTime EndEvent { get; set; }

        public int EvaluationFileId { get; set; }

        public bool ToggleChangeAxisWeight { get; set; }

        public EvaluationFile EvaluationFile { get; set; }
    }
}