using System.Collections.Generic;
using System.Threading.Tasks;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Interfaces
{
    public interface IEvaluationFileInstanceRepository
    {
        Task<IEnumerable<EvaluationFileInstance>> GetEvaluationFileInstancesByEvaluationFileId(int evaluationFileId);

        Task<IEnumerable<User>> GetUsersWithInstanceFileEvaluation(int evaluationFileId, IEnumerable<int> userIds);

        Task<IEnumerable<EvaluationFileInstance>> GetEvaluationFileInstancesToValidate(IEnumerable<int> evaluateeIds);

        Task<EvaluationFileInstance> GetEvaluationFileInstance(int id);

        Task<EvaluationFileInstance> GetEvaluationFileInstanceByUserId(int userId, int modelId);

        Task<PagedList<EvaluationFileInstance>> GetEvaluationFileInstancesForUser(CommunParams communParams);

        Task<int> GetAxisInstanceByUserIdAndAxisTitle(int evaluateeId, int modelId, string axisInstanceTitle, int parentGoalId);

        void AddEvaluationFileInstance(EvaluationFileInstance evaluationFileInstance);

        void UpdateEvaluationFileInstance(EvaluationFileInstance dbEvaluationFileInstance);

        void DeleteEvaluationFileInstance(EvaluationFileInstance evaluationFileInstance);

        Task SaveAllAsync();
    }
}