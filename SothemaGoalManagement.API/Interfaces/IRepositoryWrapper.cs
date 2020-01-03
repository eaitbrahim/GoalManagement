namespace SothemaGoalManagement.API.Interfaces
{
    public interface IRepositoryWrapper
    {
        IAxisInstanceRepository AxisInstance { get; }

        IAxisPoleRepository AxisPole { get; }

        IAxisRepository Axis { get; }

        IBehavioralSkillRepository BehavioralSkill { get; }

        IBehavioralSkillInstanceRepository BehavioralSkillInstance { get; }

        IEvaluationFileInstanceRepository EvaluationFileInstance { get; }

        IEvaluationFileRepository EvaluationFile { get; }

        IMessageRepository Message { get; }

        IPhotoRepository Photo { get; }

        IStrategyRepository Strategy { get; }

        IUserRepository User { get; }

        IUserStatusRepository UserStatus { get; }

        IPoleRepository Pole { get; }

        IDepartmentRepository Department { get; }

        IGoalTypeRepository GoalType { get; }

        IGoalRepository Goal { get; }

        IGoalEvaluationRepository GoalEvaluation { get; }
        IBehavioralSkillEvaluationRepository BehavioralSkillEvaluation { get; }

        IEvaluationFileInstanceLogRepository EvaluationFileInstanceLog { get; }

        IParametersRepository Parameters { get; }

    }
}