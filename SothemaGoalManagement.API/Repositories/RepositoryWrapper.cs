using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Interfaces;

namespace SothemaGoalManagement.API.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private DataContext _repoContext;
        private IAxisInstanceRepository _axisInstance;
        private IAxisPoleRepository _axisPole;
        private IAxisRepository _axis;
        private IBehavioralSkillRepository _behavioralSkill;
        private IBehavioralSkillInstanceRepository _behavioralSkillInstance;
        private IEvaluationFileInstanceRepository _evaluationFileInstance;
        private IEvaluationFileRepository _evaluationFile;
        private IMessageRepository _message;
        private IPhotoRepository _photo;
        private IStrategyRepository _stratgey;
        private IUserRepository _user;
        private IUserStatusRepository _userStatus;
        private IPoleRepository _pole;
        private IDepartmentRepository _department;
        private IGoalTypeRepository _goalType;
        private IGoalRepository _goal;
        private IGoalEvaluationRepository _goalEvaluation;
        private IBehavioralSkillEvaluationRepository _behavioralSkillEvaluation;
        private IEvaluationFileInstanceLogRepository _evaluationFileInstanceLog;
        private IParametersRepository _parameters;

        public IAxisInstanceRepository AxisInstance
        {
            get
            {
                if (_axisInstance == null)
                {
                    _axisInstance = new AxisInstanceRepository(_repoContext);
                }
                return _axisInstance;
            }
        }

        public IAxisPoleRepository AxisPole
        {
            get
            {
                if (_axisPole == null)
                {
                    _axisPole = new AxisPoleRepository(_repoContext);
                }
                return _axisPole;
            }
        }

        public IAxisRepository Axis
        {
            get
            {
                if (_axis == null)
                {
                    _axis = new AxisRepository(_repoContext);
                }
                return _axis;
            }
        }

        public IBehavioralSkillRepository BehavioralSkill
        {
            get
            {
                if (_behavioralSkill == null)
                {
                    _behavioralSkill = new BehavioralSkillRepository(_repoContext);
                }
                return _behavioralSkill;
            }
        }

        public IBehavioralSkillInstanceRepository BehavioralSkillInstance
        {
            get
            {
                if (_behavioralSkillInstance == null)
                {
                    _behavioralSkillInstance = new BehavioralSkillInstanceRepository(_repoContext);
                }
                return _behavioralSkillInstance;
            }
        }

        public IEvaluationFileInstanceRepository EvaluationFileInstance
        {
            get
            {
                if (_evaluationFileInstance == null)
                {
                    _evaluationFileInstance = new EvaluationFileInstanceRepository(_repoContext);
                }
                return _evaluationFileInstance;
            }
        }

        public IEvaluationFileRepository EvaluationFile
        {
            get
            {
                if (_evaluationFile == null)
                {
                    _evaluationFile = new EvaluationFileRepository(_repoContext);
                }
                return _evaluationFile;
            }
        }

        public IMessageRepository Message
        {
            get
            {
                if (_message == null)
                {
                    _message = new MessageRepository(_repoContext);
                }
                return _message;
            }
        }

        public IPhotoRepository Photo
        {
            get
            {
                if (_photo == null)
                {
                    _photo = new PhotoRepository(_repoContext);
                }
                return _photo;
            }
        }

        public IStrategyRepository Strategy
        {
            get
            {
                if (_stratgey == null)
                {
                    _stratgey = new StrategyRepository(_repoContext);
                }
                return _stratgey;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }
        }

        public IUserStatusRepository UserStatus
        {
            get
            {
                if (_userStatus == null)
                {
                    _userStatus = new UserStatusRepository(_repoContext);
                }
                return _userStatus;
            }
        }

        public IDepartmentRepository Department
        {
            get
            {
                if (_department == null)
                {
                    _department = new DepartmentRepository(_repoContext);
                }
                return _department;
            }
        }

        public IPoleRepository Pole
        {
            get
            {
                if (_pole == null)
                {
                    _pole = new PoleRepository(_repoContext);
                }
                return _pole;
            }
        }

        public IGoalTypeRepository GoalType
        {
            get
            {
                if (_goalType == null)
                {
                    _goalType = new GoalTypeRepository(_repoContext);
                }
                return _goalType;
            }
        }

        public IGoalRepository Goal
        {
            get
            {
                if (_goal == null)
                {
                    _goal = new GoalRepository(_repoContext);
                }
                return _goal;
            }
        }

        public IGoalEvaluationRepository GoalEvaluation
        {
            get
            {
                if (_goalEvaluation == null)
                {
                    _goalEvaluation = new GoalEvaluationRepository(_repoContext);
                }
                return _goalEvaluation;
            }
        }
        public IBehavioralSkillEvaluationRepository BehavioralSkillEvaluation
        {
            get
            {
                if (_behavioralSkillEvaluation == null)
                {
                    _behavioralSkillEvaluation = new BehavioralSkillEvaluationRepository(_repoContext);
                }
                return _behavioralSkillEvaluation;
            }
        }

        public IEvaluationFileInstanceLogRepository EvaluationFileInstanceLog
        {
            get
            {
                if (_evaluationFileInstanceLog == null)
                {
                    _evaluationFileInstanceLog = new EvaluationFileInstanceLogRepository(_repoContext);
                }
                return _evaluationFileInstanceLog;
            }
        }

        public IParametersRepository Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new ParametersRepository(_repoContext);
                }
                return _parameters;
            }
        }

        public RepositoryWrapper(DataContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}