using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Repositories
{
    public class GoalTypeRepository : GMRepository<GoalType>, IGoalTypeRepository
    {
        public GoalTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<GoalType>> GetGoalType()
        {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjects()
        {
            return await RepositoryContext.Projects.Where(p => p.Closed == false).ToListAsync();
        }
    }
}