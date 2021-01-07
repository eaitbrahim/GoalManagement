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
    public class GoalRepository : GMRepository<Goal>, IGoalRepository
    {
        public GoalRepository(DataContext repositoryContext) : base(repositoryContext) { }

        public async Task<Goal> GetGoal(int id)
        {
            return await FindByCondition(p => p.Id == id).Include(t => t.GoalType)
                                            .Include(a => a.AxisInstance)
                                            .ThenInclude(ai => ai.EvaluationFileInstance)
                                            .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Goal>> GetGoalChildren(int id)
        {
            return await FindByCondition(g => g.ParentGoalId == id).Include(t => t.GoalType)
                                                   .Include(a => a.AxisInstance)
                                                   .ThenInclude(ai => ai.EvaluationFileInstance)
                                                   .ThenInclude(s => s.Owner)
                                                   .ThenInclude(o => o.Photos)
                                                   .ToListAsync();
        }

        public async Task<IEnumerable<Goal>> GetGoalsByAxisInstanceIds(IEnumerable<int> axisInstanceIds)
        {
            var goalsByAxisInstanceIds = await FindByCondition(u => axisInstanceIds.Contains(u.AxisInstanceId))
                                            .Include(t => t.GoalType)
                                            .Include(a => a.AxisInstance)
                                            .ThenInclude(ai => ai.EvaluationFileInstance)
                                            .ThenInclude(s => s.Owner)
                                            .ThenInclude(o => o.Photos)
                                            .Include(ge => ge.GoalEvaluations)
                                            .ThenInclude(e => e.Evaluator)
                                            .ToListAsync();

            goalsByAxisInstanceIds.ForEach(g => g.GoalEvaluations = g.GoalEvaluations.OrderByDescending(e => e.Created).ToList());
            return goalsByAxisInstanceIds;
        }

        public async Task<User> GetGoalOwner(int goalId)
        {
            var goal = await FindByCondition(g => g.Id == goalId)
                                        .Include(g => g.AxisInstance)
                                        .ThenInclude(ai => ai.EvaluationFileInstance)
                                        .ThenInclude(s => s.Owner)
                                        .ThenInclude(o => o.Photos)
                                        .SingleOrDefaultAsync();

            return goal.AxisInstance.EvaluationFileInstance.Owner;
        }

        public async Task<IEnumerable<Goal>> GetGoalsByIds(IEnumerable<int> ids)
        {
            return await FindByCondition(u => ids.Contains(u.Id))
                                .Include(t => t.GoalType)
                                .Include(a => a.AxisInstance).ToListAsync();
        }

    public async Task<PagedList<Goal>> GetGoalsForReport(CommunParams communParams)
    {
        var goals = RepositoryContext.Goals.Include(g => g.AxisInstance)
                                            .ThenInclude(a => a.EvaluationFileInstance)
                                            .ThenInclude(s => s.Owner)
                                            .ThenInclude(o => o.Department)
                                            .ThenInclude(d => d.Pole)
                                            .AsQueryable();

        if (communParams.Year != 0)
        {
            goals = goals.Where(g => g.AxisInstance.EvaluationFileInstance.Year == communParams.Year);
        }

        if (communParams.UserToSearch != null && communParams.UserToSearch != "")
        {
            goals = goals.Where(g => g.AxisInstance.EvaluationFileInstance.Owner.FirstName.ToLower().Contains(communParams.UserToSearch.ToLower()) 
                                                || g.AxisInstance.EvaluationFileInstance.Owner.LastName.ToLower().Contains(communParams.UserToSearch.ToLower()));
        }

        if (communParams.PoleId != 0)
        {
            goals = goals.Where(s => s.AxisInstance.EvaluationFileInstance.Owner.Department.Pole.Id == communParams.PoleId);
        }

        if (communParams.UserStatusId != 0)
            {
                goals = goals.Where(s => s.AxisInstance.EvaluationFileInstance.Owner.UserStatusId == communParams.UserStatusId);
            }


        return await PagedList<Goal>.CreateAsync(goals, communParams.PageNumber, communParams.PageSize);
    }
        public void AddGoal(Goal goal)
        {
            Add(goal);
        }

        public void UpdateGoal(Goal dbGoal)
        {
            Update(dbGoal);
        }

        public void DeleteGoal(Goal goal)
        {
            Delete(goal);
        }

        public async Task SaveAllAsync()
        {
            await SaveAll();
        }
    }
}