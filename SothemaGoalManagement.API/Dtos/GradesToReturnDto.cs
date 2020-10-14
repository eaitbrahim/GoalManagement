namespace SothemaGoalManagement.API.Dtos
{
    public class GradesToReturnDto
    {
        public int SheetId { get; set; }
        public string Title { get; set; }

        public int Year { get; set; }

        public string EmployeeNumber { get; set; }
        public int GoalsGrade { get; set; }
        public int CompetencesGrade { get; set; }
        public int CompetencesPercentile { get; set; }
        public int Grade { get; set; }
    }
}