using System.Collections.Generic;
using System.Threading.Tasks;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Interfaces
{
    public interface IEvaluationFileRepository
    {
        Task<IEnumerable<EvaluationViewModel>> GetEvaluationFiles(CommunParams evaluationFileParams);

        Task<EvaluationFile> GetEvaluationFile(int id);

        Task<EvaluationFile> GetModelWithAxisPoles(int modelId);

        Task<EvaluationFile> GetEvaluationFileByStratgeyId(int strategyId);

        Task<IEnumerable<int>> GetEvaluationFileBehavioralSkillIds(int evaluationFileId);

        Task<EvaluationViewModel> GetEvaluationFileDetail(int id);

        void AddEvaluationFile(EvaluationFile evaluationFile);

        void UpdateEvaluationFile(EvaluationFile dbEvaluationFile);

        void DeleteEvaluationFile(EvaluationFile evaluationFile);

        Task SaveAllAsync();

    }
}