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
    public class BehavioralSkillInstanceRepository : GMRepository<BehavioralSkillInstance>, IBehavioralSkillInstanceRepository
    {
        public BehavioralSkillInstanceRepository(DataContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<IEnumerable<BehavioralSkillInstance>> GetBehavioralSkillInstancesByBSIds(IEnumerable<int> bsIds)
        {
            return await RepositoryContext.BehavioralSkillInstances.Where(bsi => bsIds.Contains(bsi.BehavioralSkillId)).ToListAsync();
        }

        public async Task<IEnumerable<BehavioralSkillInstance>> GetBehavioralSkillInstancesBySheetId(int sheetId)
        {
            var behavioralSkillIds = await RepositoryContext.EvaluationFileInstanceBehavioralSkillInstances
                                                            .Where(efi => efi.EvaluationFileInstanceId == sheetId)
                                                            .Select(efi => efi.BehavioralSkillInstanceId)
                                                            .ToListAsync();
            return await RepositoryContext.BehavioralSkillInstances
                                            .Where(bsi => behavioralSkillIds.Contains(bsi.Id))
                                            .ToListAsync();
        }

        public void AddBehavioralSkillInstance(BehavioralSkillInstance behavioralSkillInstance)
        {
            Add(behavioralSkillInstance);
        }

        public void UpdateBehavioralSkillInstance(BehavioralSkillInstance dbBehavioralSkillInstance)
        {
            Update(dbBehavioralSkillInstance);
        }

        public void DeleteBehavioralSkillInstance(BehavioralSkillInstance behavioralSkillInstance)
        {
            Delete(behavioralSkillInstance);

        }

        public async Task SaveAllAsync()
        {
            await SaveAll();
        }
    }
}