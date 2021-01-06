using System.Collections.Generic;
using System.Threading.Tasks;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Interfaces
{
    public interface IGoalRepository
    {
        Task<Goal> GetGoal(int id);

        Task<IEnumerable<Goal>> GetGoalsByAxisInstanceIds(IEnumerable<int> axisInstanceIds);

        Task<IEnumerable<Goal>> GetGoalChildren(int id);

        Task<IEnumerable<Goal>> GetGoalsByIds(IEnumerable<int> ids);

        Task<User> GetGoalOwner(int goalId);

        Task<PagedList<Goal>> GetGoalsForReport(CommunParams communParams);

        void AddGoal(Goal goal);

        void UpdateGoal(Goal dbGoal);

        void DeleteGoal(Goal goal);

        Task SaveAllAsync();

    }
}