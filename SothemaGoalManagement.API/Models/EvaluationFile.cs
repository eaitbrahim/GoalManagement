using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SothemaGoalManagement.API.Helpers;

namespace SothemaGoalManagement.API.Models
{
    public class EvaluationFile
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public DateTime Created { get; set; }

        public User Owner { get; set; }

        public int OwnerId { get; set; }

        public ICollection<EvaluationFileBehavioralSkill> BehavioralSkills { get; set; }
        public ICollection<Parameters> Parameters { get; set; }

        public int StrategyId { get; set; }

        public Strategy Strategy { get; set; }

        public bool Sealed { get; set; }

        public DateTime SealedDate { get; set; }


        public EvaluationFile()
        {
            BehavioralSkills = new Collection<EvaluationFileBehavioralSkill>();
            Parameters = new Collection<Parameters>();
        }
    }
}