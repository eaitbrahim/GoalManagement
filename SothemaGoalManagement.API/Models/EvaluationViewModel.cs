using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SothemaGoalManagement.API.Models
{
    public class EvaluationViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public Strategy Strategy { get; set; }

        public int CreatedById { get; set; }

        public string CreatedByName { get; set; }

        public ICollection<BehavioralSkill> BehavioralSkills { get; set; }

        public ICollection<Axis> AxisList { get; set; }

        public DateTime Created { get; set; }

        public string Status { get; set; }

        public bool Sealed { get; set; }

        public DateTime SealedDate { get; set; }

        public ICollection<Parameters> Parameters { get; set; }

        public EvaluationViewModel()
        {
            BehavioralSkills = new Collection<BehavioralSkill>();
            AxisList = new Collection<Axis>();
            Parameters = new Collection<Parameters>();
        }
    }
}