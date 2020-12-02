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

namespace SothemaGoalManagement.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class SheetController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public SheetController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper, UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySheetsForUser(int userId, [FromQuery]CommunParams communParams)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                communParams.OwnerId = userId;
                var sheetsFromRepo = await _repo.EvaluationFileInstance.GetEvaluationFileInstancesForUser(communParams);
                var sheets = _mapper.Map<IEnumerable<EvaluationFileInstanceToReturnDto>>(sheetsFromRepo);

                Response.AddPagination(sheetsFromRepo.CurrentPage, sheetsFromRepo.PageSize, sheetsFromRepo.TotalCount, sheetsFromRepo.TotalPages);

                return Ok(sheets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMySheetsForUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("mySheet/{id}")]
        public async Task<IActionResult> GetMySheetDetailForUser(int userId, int id)
        {
            try
            {
                var evaluationFileInstanceFromRepo = await _repo.EvaluationFileInstance.GetEvaluationFileInstance(id);
                if (evaluationFileInstanceFromRepo == null) return NotFound();
                if (!await IsItAllowed(evaluationFileInstanceFromRepo.OwnerId)) return Unauthorized();

                var evaluationFileInstanceToReturn = _mapper.Map<EvaluationFileInstanceToReturnDto>(evaluationFileInstanceFromRepo);

                return Ok(evaluationFileInstanceToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMySheetDetailForUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("myCollaboratorsSheets")]
        public async Task<IActionResult> GetMyCollaboratorsSheets(int userId, [FromQuery]CommunParams communParams)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var evaluatees = await _repo.User.LoadEvaluatees(userId);
                IList<int> evaluateeIds = new List<int>();
                foreach (var evaluatee in evaluatees)
                {
                    evaluateeIds.Add(evaluatee.Id);
                }
                var sheetsTovalidateFromRepo = await _repo.EvaluationFileInstance.GetEvaluationFileInstancesToValidate(evaluateeIds, communParams);
                var sheetsToValidate = _mapper.Map<IEnumerable<EvaluationFileInstanceToReturnDto>>(sheetsTovalidateFromRepo);
                sheetsToValidate = await SetGoalsStatus((List<EvaluationFileInstanceToReturnDto>)sheetsToValidate);
                return Ok(sheetsToValidate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMyCollaboratorsSheets endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPatch("{axisInstanceId}/{userWeight}")]
        public async Task<IActionResult> UpdateAxisInstance(int userId, int axisInstanceId, int userWeight)
        {
            try
            {
                var userFromRepo = await _repo.User.GetUser(userId, false);
                if (userFromRepo.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var axisIntsnaceFromRepo = await _repo.AxisInstance.GetAxisInstance(axisInstanceId);
                if (axisIntsnaceFromRepo != null)
                {
                    // Validate goal's status
                    IList<int> axisInstanceIds = new List<int>() { axisInstanceId };
                    List<Goal> goals = (List<Goal>)await _repo.Goal.GetGoalsByAxisInstanceIds(axisInstanceIds);
                    // if(goals == null || goals.Count == 0)
                    // {
                    //     return BadRequest("Pas d'objectif défini pour cet axe!");
                    // }
                    
                    if (goals.First().Status == Constants.PUBLISHED)
                    {
                        return BadRequest("Trop tard! Les objectifs sont déjà validés.");
                    }

                    //Proceed with update
                    var oldUserWeight = axisIntsnaceFromRepo.UserWeight;
                    axisIntsnaceFromRepo.UserWeight = userWeight;
                    _repo.AxisInstance.UpdateAxisInstance(axisIntsnaceFromRepo);
                    await _repo.AxisInstance.SaveAllAsync();

                    // Log the update of user's weight
                    var user = _repo.User.GetUser(userId, true).Result;
                    var efil = new EvaluationFileInstanceLog
                    {
                        Title = axisIntsnaceFromRepo.EvaluationFileInstance.Title,
                        Created = DateTime.Now,
                        Log = $"La pondération de l\'employée est modifié de {oldUserWeight} à {userWeight} pour '{axisIntsnaceFromRepo.EvaluationFileInstance.Title}' par {user.FirstName} {user.LastName}."
                    };
                    _repo.EvaluationFileInstanceLog.AddEvaluationFileInstanceLog(efil);
                    await _repo.EvaluationFileInstanceLog.SaveAllAsync();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateAxisInstance endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{addFinalEvaluation}/{sheetId}")]
        public async Task<IActionResult> AddFinalEvaluation(int userId, int sheetId, FinalEvaluationDto finalEvaluation)
        {
            try
            {
                var sheetFromRepo = await _repo.EvaluationFileInstance.GetEvaluationFileInstance(sheetId);
                if (sheetFromRepo == null) return NotFound();

                if (!await IsItAllowed(sheetFromRepo.OwnerId)) return Unauthorized();

                var processUpdate = false;

                // Case of evaluation by Owner
                if (!string.IsNullOrEmpty(finalEvaluation.OwnerComment))
                {
                    sheetFromRepo.OwnerValidationDateTime = finalEvaluation.OwnerValidationDateTime;
                    sheetFromRepo.OwnerComment = finalEvaluation.OwnerComment;
                    if (sheetFromRepo.ValidatorId > 0)
                    {
                        sheetFromRepo.Status = Constants.PUBLISHED;
                    }

                    processUpdate = true;
                }

                // Case of evaluation by Evaluator
                if (!string.IsNullOrEmpty(finalEvaluation.ValidatorComment))
                {
                    sheetFromRepo.ValidatorValidationDateTime = finalEvaluation.ValidatorValidationDateTime;
                    sheetFromRepo.ValidatorComment = finalEvaluation.ValidatorComment;
                    sheetFromRepo.ValidatorId = finalEvaluation.ValidatorId;
                    if (sheetFromRepo.OwnerComment != null)
                    {
                        sheetFromRepo.Status = Constants.PUBLISHED;
                    }
                    processUpdate = true;
                }

                if (processUpdate)
                {
                    _repo.EvaluationFileInstance.UpdateEvaluationFileInstance(sheetFromRepo);
                    await _repo.EvaluationFileInstance.SaveAllAsync();

                    // Log the update of user's weight
                    var user = _repo.User.GetUser(userId, true).Result;
                    var efil = new EvaluationFileInstanceLog
                    {
                        Title = sheetFromRepo.Title,
                        Created = DateTime.Now,
                        Log = $"Une évaluation finale a été sauveguardée avec succés' par {user.FirstName} {user.LastName}."
                    };
                    _repo.EvaluationFileInstanceLog.AddEvaluationFileInstanceLog(efil);
                    await _repo.EvaluationFileInstanceLog.SaveAllAsync();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddFinalEvaluation endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task<IEnumerable<EvaluationFileInstanceToReturnDto>> SetGoalsStatus(List<EvaluationFileInstanceToReturnDto> sheets)
        {
            IList<int> axisInstanceIds = new List<int>();
            foreach (var sheet in sheets)
            {
                foreach (var axisInstance in sheet.AxisInstances)
                {
                    axisInstanceIds.Add(axisInstance.Id);
                    break;
                }
            }

            List<Goal> goals = (List<Goal>)await _repo.Goal.GetGoalsByAxisInstanceIds(axisInstanceIds);
            for (int i = 0; i < axisInstanceIds.Count(); i++)
            {
                var goal = goals.FirstOrDefault(g => g.AxisInstanceId == axisInstanceIds[i]);
                if (goal == null)
                {
                    sheets[i].GoalsStatus = Constants.NOTSTARTED;
                }
                else
                {
                    sheets[i].GoalsStatus = goal.Status;
                }

            }

            return sheets;
        }

        private async Task<bool> IsItAllowed(int ownerId, string action = "read")
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (ownerId != currentUserId)
            {
                var evaluators = await _repo.User.LoadEvaluators(ownerId);
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