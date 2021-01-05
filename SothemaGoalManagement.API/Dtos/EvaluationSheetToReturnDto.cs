using System;
using System.Collections.Generic;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Dtos
{
    public class EvaluationSheetToReturnDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string EmployeeNumber { get; set; }

        public int Year { get; set; }

        public string PoleName { get; set; }
        public string GoalsStatus { get; set; }
        public DateTime ValidatorValidationDateTime { get; set; }

        public string GoalsTotalGrade { get; set; }

        public int BehavioralSkillsGrade { get; set; }

        public ICollection<AxisInstance> AxisInstances { get; set; }

    }
}