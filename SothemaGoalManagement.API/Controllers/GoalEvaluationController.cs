using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalEvaluationManagement.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class GoalEvaluationController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public GoalEvaluationController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper, UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
            _userManager = userManager;
        }

        [HttpGet("{id}", Name = "GetGoalEvaluation")]
        public async Task<IActionResult> GetGoalEvaluation(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var goalEvaluationFromRepo = await _repo.GoalEvaluation.GetGoalEvaluation(id);

                if (goalEvaluationFromRepo == null) return NotFound();
                var goalEvaluationToReturn = _mapper.Map<GoalEvaluationToReturnDto>(goalEvaluationFromRepo);

                return Ok(goalEvaluationToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoalEvaluation endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("goalEvaluations/{goalId}")]
        public async Task<IActionResult> GetGoalEvaluationsByGoalId(int userId, int goalId)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var goalEvaluationList = await _repo.GoalEvaluation.GetGoalEvaluationsByGoalId(goalId);
                var goalEvaluationToReturn = _mapper.Map<IEnumerable<GoalEvaluationToReturnDto>>(goalEvaluationList);

                return Ok(goalEvaluationToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoalEvaluationsByGoalId endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("createGoalEvaluation")]
        public async Task<IActionResult> CreateGoalEvaluation(int userId, GoalEvaluationForCreationDto goalEvaluationCreationDto)
        {
            try
            {
                var goalOwner = await _repo.Goal.GetGoalOwner(goalEvaluationCreationDto.GoalId);
                if (!await IsItAllowed(goalOwner.Id, "write")) return Unauthorized();

                // Create a new goalEvaluation
                var goalEvaluation = _mapper.Map<GoalEvaluation>(goalEvaluationCreationDto);
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) goalEvaluation.SelfEvaluation = false;
                else goalEvaluation.SelfEvaluation = true;
                _repo.GoalEvaluation.AddGoalEvaluation(goalEvaluation);
                await _repo.GoalEvaluation.SaveAllAsync();

                return CreatedAtRoute("GetGoalEvaluation", new { id = goalEvaluation.Id }, goalEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEvaluationFile endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task<bool> IsUserHasEvaluator(int userId)
        {
            var evaluators = await _repo.User.LoadEvaluators(userId);
            if (evaluators == null || evaluators.Count() == 0) return false;
            return true;
        }

        private async Task<bool> IsItAllowed(int goalOwnerId, string action = "read")
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (goalOwnerId != currentUserId)
            {
                var evaluators = await _repo.User.LoadEvaluators(goalOwnerId);
                var evaluator = evaluators.FirstOrDefault(e => e.Id == currentUserId);
                if (evaluator == null)
                {
                    if (action == "read")
                    {
                        // Check if current user has allowed roles:
                        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
                        var roles = await _userManager.GetRolesAsync(currentUser);
                        foreach (var role in roles)
                        {
                            if (role == Constants.HR || role == Constants.HRD) return true;

                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}