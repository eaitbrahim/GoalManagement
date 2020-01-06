using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Repositories
{
    public class EvaluationFileRepository : GMRepository<EvaluationFile>, IEvaluationFileRepository
    {
        public EvaluationFileRepository(DataContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<IEnumerable<EvaluationViewModel>> GetEvaluationFiles(CommunParams evaluationFileParams)
        {
            var evaluationFilesWithBehavioralSkills = (from evaluationFile in RepositoryContext.EvaluationFiles.Include(ef => ef.Strategy).Include(ef => ef.Owner)
                                                       select new EvaluationViewModel()
                                                       {
                                                           Id = evaluationFile.Id,
                                                           Title = evaluationFile.Title,
                                                           Year = evaluationFile.Year,
                                                           Strategy = evaluationFile.Strategy,
                                                           CreatedById = evaluationFile.OwnerId,
                                                           CreatedByName = evaluationFile.Owner.FirstName.FullName(evaluationFile.Owner.LastName),
                                                           BehavioralSkills = (from evaluationFileBehavioralSkill in evaluationFile.BehavioralSkills
                                                                               join bs in RepositoryContext.BehavioralSkills on evaluationFileBehavioralSkill.BehavioralSkillId equals bs.Id
                                                                               select bs).ToList(),
                                                           Created = evaluationFile.Created,
                                                           Status = evaluationFile.Status,
                                                           Sealed = evaluationFile.Sealed,
                                                           SealedDate = evaluationFile.SealedDate
                                                       }).OrderByDescending(u => u.Created)
                                  .AsQueryable();

            switch (evaluationFileParams.Status)
            {
                case Constants.PUBLISHED:
                    evaluationFilesWithBehavioralSkills = evaluationFilesWithBehavioralSkills.Where(s => s.Status == Constants.PUBLISHED);
                    break;
                case Constants.DRAFT:
                    evaluationFilesWithBehavioralSkills = evaluationFilesWithBehavioralSkills.Where(s => s.Status == Constants.DRAFT && s.CreatedById == evaluationFileParams.OwnerId);
                    break;
                case Constants.REVIEW:
                    evaluationFilesWithBehavioralSkills = evaluationFilesWithBehavioralSkills.Where(s => s.Status == Constants.REVIEW);
                    break;
                case Constants.ARCHIVED:
                    evaluationFilesWithBehavioralSkills = evaluationFilesWithBehavioralSkills.Where(s => s.Status == Constants.ARCHIVED);
                    break;
                default:
                    evaluationFilesWithBehavioralSkills = evaluationFilesWithBehavioralSkills.Where(s => (s.Status == Constants.DRAFT && s.CreatedById == evaluationFileParams.OwnerId)
                    || s.Status == Constants.REVIEW || s.Status == Constants.PUBLISHED || s.Status == Constants.ARCHIVED);
                    break;
            }

            return await evaluationFilesWithBehavioralSkills.ToListAsync();
        }

        public async Task<EvaluationFile> GetEvaluationFile(int id)
        {
            return await RepositoryContext.EvaluationFiles.Include(ef => ef.Strategy).SingleOrDefaultAsync(ef => ef.Id == id);
        }

        public async Task<EvaluationFile> GetModelWithAxisPoles(int modelId)
        {
            return await RepositoryContext.EvaluationFiles.Include(m => m.Strategy).ThenInclude(s => s.AxisList)
                                                           .ThenInclude(a => a.AxisPoles).SingleOrDefaultAsync(m => m.Id == modelId);
        }

        public async Task<EvaluationFile> GetEvaluationFileByStratgeyId(int strategyId)
        {
            return await RepositoryContext.EvaluationFiles.SingleOrDefaultAsync(ef => ef.StrategyId == strategyId);
        }

        public async Task<EvaluationViewModel> GetEvaluationFileDetail(int id)
        {
            return await (from evaluationFile in RepositoryContext.EvaluationFiles.Include(ef => ef.Strategy)
                                                                         .ThenInclude(s => s.AxisList)
                                                                         .ThenInclude(a => a.AxisPoles)
                                                                         .Include(ef => ef.Owner)
                                                                         .Include(ef => ef.Parameters)
                          select new EvaluationViewModel
                          {
                              Id = evaluationFile.Id,
                              Title = evaluationFile.Title,
                              Year = evaluationFile.Year,
                              Strategy = evaluationFile.Strategy,
                              CreatedById = evaluationFile.OwnerId,
                              CreatedByName = evaluationFile.Owner.FirstName.FullName(evaluationFile.Owner.LastName),
                              BehavioralSkills = (from evaluationFileBehavioralSkill in evaluationFile.BehavioralSkills
                                                  join bs in RepositoryContext.BehavioralSkills on evaluationFileBehavioralSkill.BehavioralSkillId equals bs.Id
                                                  select bs).ToList(),
                              AxisList = evaluationFile.Strategy.AxisList,
                              Created = evaluationFile.Created,
                              Status = evaluationFile.Status,
                              Sealed = evaluationFile.Sealed,
                              SealedDate = evaluationFile.SealedDate,
                              Parameters = evaluationFile.Parameters
                          }).SingleOrDefaultAsync(ef => ef.Id == id);
        }

        public async Task<IEnumerable<int>> GetEvaluationFileBehavioralSkillIds(int evaluationFileId)
        {
            return await RepositoryContext.EvaluationFileBehavioralSkills.Where(efbs => efbs.EvaluationFileId == evaluationFileId).Select(efbs => efbs.BehavioralSkillId).ToListAsync();
        }

        public void AddEvaluationFile(EvaluationFile evaluationFile)
        {
            Add(evaluationFile);
        }

        public void UpdateEvaluationFile(EvaluationFile dbEvaluationFile)
        {
            Update(dbEvaluationFile);
        }

        public void DeleteEvaluationFile(EvaluationFile evaluationFile)
        {
            Delete(evaluationFile);
        }

        public async Task SaveAllAsync()
        {
            await SaveAll();
        }
    }
}