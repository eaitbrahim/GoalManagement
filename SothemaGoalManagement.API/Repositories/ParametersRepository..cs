using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Repositories
{
    public class ParametersRepository : GMRepository<Parameters>, IParametersRepository
    {
        public ParametersRepository(DataContext repositoryContext) : base(repositoryContext) { }

        public async Task<Parameters> GetParameters(int id)
        {
            return await FindByCondition(p => p.Id == id).FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Parameters>> GetParametersByModeId(int modelId)
        {
            return await FindByCondition(p => p.EvaluationFileId == modelId).ToListAsync();
        }

        public void AddParameters(Parameters Parameters)
        {
            Add(Parameters);
        }


        public void DeleteParameters(Parameters Parameters)
        {
            Delete(Parameters);
        }

        public async Task SaveAllAsync()
        {
            await SaveAll();
        }
    }
}