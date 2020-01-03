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
            var Parameters = await RepositoryContext.Parameters.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            return Parameters;
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