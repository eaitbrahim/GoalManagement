using System.ComponentModel.DataAnnotations;

namespace SothemaGoalManagement.API.Dtos
{
    public class BehavioralSkillForUpdateDto
    {
        [Required] public int Id { get; set; }
        [Required] public string Skill { get; set; }
        [Required] public string Definition { get; set; }
        [Required] public string LevelOne { get; set; }
        [Required] public string LevelTwo { get; set; }
        [Required] public string LevelThree { get; set; }
        [Required] public string LevelFour { get; set; }
        [Required] public string LevelOneGrade { get; set; }
        [Required] public string LevelTwoGrade { get; set; }
        [Required] public string LevelThreeGrade { get; set; }
        [Required] public string LevelFourGrade { get; set; }
        public string LevelOneDescription { get; set; }
        public string LevelTwoDescription { get; set; }
        public string LevelThreeDescription { get; set; }
        public string LevelFourDescription { get; set; }
        [Required] public string Status { get; set; }
    }
}