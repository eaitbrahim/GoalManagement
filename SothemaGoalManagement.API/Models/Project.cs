namespace SothemaGoalManagement.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int GoalTypeId { get; set; }

        public bool Closed { get; set; }
    }
}