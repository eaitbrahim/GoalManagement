using System;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Dtos
{
    public class StrategyForListDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string OwnerName { get; set; }

        public int OwnerId { get; set; }

        public DateTime Created { get; set; }

        public bool Sealed { get; set; }

        public DateTime SealedDate { get; set; }


    }
}