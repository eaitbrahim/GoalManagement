using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SothemaGoalManagement.API.Dtos
{
    public class EvaluationFileInstanceToReturnDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public string GoalsStatus { get; set; }

        public string StrategyTitle { get; set; }
        public string StrategyDescription { get; set; }

        public string OwnerName { get; set; }

        public int OwnerId { get; set; }

        public int EvaluationFileId { get; set; }

        public string PhotoUrl { get; set; }
        public string OwnerTitle { get; set; }
        public string EmployeeNumber { get; set; }

        public DateTime Created { get; set; }

        public ICollection<AxisInstanceToReturnDto> AxisInstances { get; set; }

        public string OwnerComment { get; set; }
        public string ValidatorComment { get; set; }

        public DateTime OwnerValidationDateTime { get; set; }
        public DateTime ValidatorValidationDateTime { get; set; }
        public int ValidatorId { get; set; }

        public EvaluationFileInstanceToReturnDto()
        {
            AxisInstances = new Collection<AxisInstanceToReturnDto>();
        }

    }
}