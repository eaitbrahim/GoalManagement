using System.Threading.Tasks;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Interfaces
{
    public interface IParametersRepository
    {
        Task<Parameters> GetParameters(int id);

        void AddParameters(Parameters parameters);

        void DeleteParameters(Parameters parameters);

        Task SaveAllAsync();

    }
}