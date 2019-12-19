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
    public class EvaluationFileInstanceRepository : GMRepository<EvaluationFileInstance>, IEvaluationFileInstanceRepository
    {
        public EvaluationFileInstanceRepository(DataContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<EvaluationFileInstance>> GetEvaluationFileInstancesByEvaluationFileId(int evaluationFileId)
        {
            return await RepositoryContext.EvaluationFileInstances.Include(efi => efi.AxisInstances)
                                                        .Include(efi => efi.Owner).Include(efi => efi.Owner)
                                                        .ThenInclude(u => u.Department).ThenInclude(d => d.Pole)
                                                        .Where(efi => efi.EvaluationFileId == evaluationFileId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithInstanceFileEvaluation(int evaluationFileId, IEnumerable<int> userIds)
        {
            return await RepositoryContext.EvaluationFileInstances.Include(efi => efi.Owner)
                                                         .Where(efi => efi.EvaluationFileId == evaluationFileId &&
                                                                        userIds.Contains(efi.OwnerId))
                                                         .Select(efi => efi.Owner)
                                                         .ToListAsync();
        }

        public async Task<EvaluationFileInstance> GetEvaluationFileInstance(int id)
        {
            return await FindByCondition(efi => efi.Id == id).Include(efi => efi.Owner).ThenInclude(p => p.Photos).Include(efi => efi.AxisInstances).SingleOrDefaultAsync();
        }

        public async Task<EvaluationFileInstance> GetEvaluationFileInstanceByUserId(int userId, int model)
        {
            return await FindByCondition(efi => efi.OwnerId == userId && efi.EvaluationFileId == model).SingleOrDefaultAsync();
        }

        public async Task<PagedList<EvaluationFileInstance>> GetEvaluationFileInstancesForUser(CommunParams communParams)
        {
            var sheets = FindByCondition(s => s.OwnerId == communParams.OwnerId).Include(s => s.Owner).AsQueryable();
            sheets = sheets.OrderByDescending(d => d.Year);
            return await PagedList<EvaluationFileInstance>.CreateAsync(sheets, communParams.PageNumber, communParams.PageSize);
        }

        public async Task<IEnumerable<EvaluationFileInstance>> GetEvaluationFileInstancesToValidate(IEnumerable<int> evaluateeIds)
        {
            return await RepositoryContext.EvaluationFileInstances.Include(efi => efi.AxisInstances)
                                                                .Include(efi => efi.Owner).ThenInclude(p => p.Photos)
                                                                .Include(efi => efi.Owner).ThenInclude(u => u.Department).ThenInclude(d => d.Pole)
                                                                .Where(efi => evaluateeIds.Contains(efi.OwnerId)
                                                                                && (efi.Status == Constants.DRAFT || efi.Status == Constants.REVIEW))
                                                                .ToListAsync();
        }

        public async Task<int> GetAxisInstanceByUserIdAndAxisTitle(int evaluateeId, int model, string axisInstanceTitle, int parentGoalId)
        {
            var sheetFromRepo = await RepositoryContext.EvaluationFileInstances.Include(efi => efi.AxisInstances)
                                                                                .ThenInclude(g => g.Goals)
                                                                                .SingleOrDefaultAsync(s => s.OwnerId == evaluateeId && s.EvaluationFileId == model);
            if (sheetFromRepo != null)
            {
                foreach (var axisInstance in sheetFromRepo.AxisInstances)
                {
                    var goals = axisInstance.Goals.Where(g => g.Status == Constants.PUBLISHED || g.Status == Constants.ARCHIVED).ToList();
                    if (goals != null && goals.Count > 0) break;

                    if (axisInstance.Title == axisInstanceTitle)
                    {
                        return axisInstance.Id;
                    }
                }
            }
            return 0;
        }

        public void AddEvaluationFileInstance(EvaluationFileInstance evaluationFileInstance)
        {
            Add(evaluationFileInstance);
        }

        public void UpdateEvaluationFileInstance(EvaluationFileInstance dbEvaluationFileInstance)
        {
            Update(dbEvaluationFileInstance);
        }

        public void DeleteEvaluationFileInstance(EvaluationFileInstance evaluationFileInstance)
        {
            Delete(evaluationFileInstance);
        }

        public async Task SaveAllAsync()
        {
            await SaveAll();
        }
    }
}