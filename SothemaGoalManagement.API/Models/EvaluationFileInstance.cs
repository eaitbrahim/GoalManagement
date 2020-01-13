using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SothemaGoalManagement.API.Helpers;

namespace SothemaGoalManagement.API.Models
{
    public class EvaluationFileInstance
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public DateTime Created { get; set; }

        public User Owner { get; set; }

        public int OwnerId { get; set; }

        public ICollection<EvaluationFileInstanceBehavioralSkillInstance> BehavioralSkillInstances { get; set; }

        public ICollection<AxisInstance> AxisInstances { get; set; }


        public string StrategyTitle { get; set; }

        public string StrategyDescription { get; set; }

        public int EvaluationFileId { get; set; }

        public string OwnerComment { get; set; }
        public string ValidatorComment { get; set; }

        public DateTime OwnerValidationDateTime { get; set; }
        public DateTime ValidatorValidationDateTime { get; set; }
        public int ValidatorId { get; set; }

        public EvaluationFile EvaluationFile { get; set; }
        public EvaluationFileInstance()
        {
            BehavioralSkillInstances = new Collection<EvaluationFileInstanceBehavioralSkillInstance>();
            AxisInstances = new Collection<AxisInstance>();
        }
    }
}