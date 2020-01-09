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
    public class BehavioralSkillEvaluationController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public BehavioralSkillEvaluationController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper, UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
            _userManager = userManager;
        }

        [HttpGet("{id}", Name = "GetBehavioralSkillEvaluation")]
        public async Task<IActionResult> GetBehavioralSkillEvaluation(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var behavioralSkillEvaluationFromRepo = await _repo.BehavioralSkillEvaluation.GetBehavioralSkillEvaluation(id);

                if (behavioralSkillEvaluationFromRepo == null) return NotFound();
                var behavioralSkillEvaluationToReturn = _mapper.Map<BehavioralSkillEvaluationToReturnDto>(behavioralSkillEvaluationFromRepo);

                return Ok(behavioralSkillEvaluationToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBehavioralSkillEvaluation endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("behavioralSkillInstancesForSheet/{sheetId}")]
        public async Task<IActionResult> GetBehavioralSkillInstancesForSheet(int userId, int sheetId)
        {
            try
            {
                if (!await IsItAllowed(userId)) return Unauthorized();

                var behavioralSkillInstancesFromRepo = await _repo.BehavioralSkillInstance.GetBehavioralSkillInstancesBySheetId(sheetId);
                var behavioralSkillInstanceEvaluationsFromRepo = await _repo.BehavioralSkillEvaluation.GetBehavioralSkillEvaluationsBySheetId(sheetId);
                foreach (var bsi in behavioralSkillInstancesFromRepo)
                {
                    foreach (var bse in behavioralSkillInstanceEvaluationsFromRepo)
                    {
                        if (bsi.Id == bse.BehavioralSkillInstanceId)
                        {
                            bsi.BehavioralSkillEvaluations.Add(bse);
                        }
                    }
                }
                var behavioralSkillInstancesToReturn = _mapper.Map<IEnumerable<BehavioralSkillToReturnDto>>(behavioralSkillInstancesFromRepo);
                foreach (var behavioralSkillInstanceToReturn in behavioralSkillInstancesToReturn)
                {
                    var latestEval = behavioralSkillInstanceToReturn.BehavioralSkillEvaluations.OrderByDescending(bsie => bsie.Created).FirstOrDefault();
                    behavioralSkillInstanceToReturn.BehavioralSkillGrade = latestEval != null ? latestEval.Grade : 0;
                    behavioralSkillInstanceToReturn.BehavioralSkillLevel = latestEval != null ? latestEval.Level : "";
                }

                return Ok(behavioralSkillInstancesToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBehavioralSkillInstancesForSheet endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("createBehavioralSkillEvaluation")]
        public async Task<IActionResult> createBehavioralSkillEvaluation(int userId, IEnumerable<BehavioralSkillEvaluationForCreationDto> behavioralSkillEvaluationsCreationDto)
        {
            try
            {
                if (!await IsItAllowed(userId, "write")) return Unauthorized();

                // Create a new behavioralSkillEvaluation
                foreach (var behavioralSkillEvaluationCreationDto in behavioralSkillEvaluationsCreationDto)
                {
                    var behavioralSkillEvaluation = _mapper.Map<BehavioralSkillEvaluation>(behavioralSkillEvaluationCreationDto);
                    behavioralSkillEvaluation.EvaluatorId = userId;
                    _repo.BehavioralSkillEvaluation.AddBehavioralSkillEvaluation(behavioralSkillEvaluation);
                }

                await _repo.BehavioralSkillEvaluation.SaveAllAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside createBehavioralSkillEvaluation endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        private async Task<bool> IsItAllowed(int userId, string action = "read")
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != currentUserId)
            {
                var evaluators = await _repo.User.LoadEvaluators(userId);
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