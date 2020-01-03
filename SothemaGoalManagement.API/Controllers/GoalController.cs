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
    public class GoalController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GoalController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper, UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
            _userManager = userManager;
        }

        [HttpGet("{id}", Name = "GetGoal")]
        public async Task<IActionResult> GetGoal(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var goalFromRepo = await _repo.Goal.GetGoal(id);

                if (goalFromRepo == null) return NotFound();
                var goalToReturn = _mapper.Map<GoalToReturnDto>(goalFromRepo);

                return Ok(goalToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("goalWithChildren/{goalId}")]
        public async Task<IActionResult> GetGoalWithChildren(int userId, int goalId)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var goalFromRepo = await _repo.Goal.GetGoal(goalId);

                if (goalFromRepo == null) return NotFound();
                var goalWithChildrenToReturn = _mapper.Map<GoalWithChildrenToReturnDto>(goalFromRepo);
                var childrenFromRepo = await _repo.Goal.GetGoalChildren(goalFromRepo.Id);

                if (childrenFromRepo.Count() > 0)
                {
                    goalWithChildrenToReturn.Children = _mapper.Map<ICollection<GoalWithChildrenToReturnDto>>(childrenFromRepo);
                }

                return Ok(goalWithChildrenToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("goalTypes")]
        public async Task<IActionResult> GetGoalTypes(int userId)
        {
            try
            {
                var goalTypeList = await _repo.GoalType.GetGoalType();
                var goalTypeToReturn = _mapper.Map<IEnumerable<GoalTypeToReturnDto>>(goalTypeList);

                return Ok(goalTypeToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoalType endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects(int userId)
        {
            try
            {
                var projectsFromRepo = await _repo.GoalType.GetProjects();
                var projectToReturn = _mapper.Map<IEnumerable<ProjectToReturnDto>>(projectsFromRepo);

                return Ok(projectToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoalType endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> GetGoalsForAxis(int userId, IEnumerable<int> axisInstanceIds)
        {
            try
            {
                if (!await IsItAllowed(userId)) return Unauthorized();
                var goalsGroupedByAxisInstanceList = await GetAxisInstancesWithGoals(axisInstanceIds);

                return Ok(goalsGroupedByAxisInstanceList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGoals endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("{modelId}/cascadeGoal/{sheetId}")]
        public async Task<IActionResult> CascadeGoal(int userId, int sheetId, int modelId, IEnumerable<GoalForCascadeDto> goalsCascadeDto)
        {
            try
            {
                if (goalsCascadeDto.Count() > 0)
                {
                    var sheet = await _repo.EvaluationFileInstance.GetEvaluationFileInstance(sheetId);
                    if (sheet.OwnerId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                    // Make sure evaluatees are evaluated by the claimed evaluator 
                    var evaluateesFromRepo = await _repo.User.LoadEvaluatees(sheet.OwnerId);
                    foreach (var goalCascadeDto in goalsCascadeDto)
                    {
                        if (!evaluateesFromRepo.Any(e => e.Id == goalCascadeDto.EvaluateeId))
                        {
                            return BadRequest("Un des utilisateurs sélectionnés ne figure pas dans votre liste pour évaluer!");
                        }
                    }

                    // Set not published or archived axis instance id foreach evaluatee
                    foreach (var goalCascadeDto in goalsCascadeDto)
                    {
                        var axisInstanceId = _repo.EvaluationFileInstance.GetAxisInstanceByUserIdAndAxisTitle(goalCascadeDto.EvaluateeId, modelId, goalCascadeDto.AxisInstanceTitle, goalCascadeDto.ParentGoalId).Result;
                        goalCascadeDto.GoalForCreationDto.AxisInstanceId = axisInstanceId;
                    }

                    // Create a new goal for each evaluatee who has an axis instance
                    foreach (var goalCascadeDto in goalsCascadeDto)
                    {
                        if (goalCascadeDto.GoalForCreationDto.AxisInstanceId != 0)
                        {
                            var goal = _mapper.Map<Goal>(goalCascadeDto.GoalForCreationDto);
                            goal.ParentGoalId = goalCascadeDto.ParentGoalId;
                            _repo.Goal.AddGoal(goal);
                        }
                    }

                    await _repo.Goal.SaveAllAsync();

                    // Log new goal has been assigned by the evaluator
                    var evaluateeIds = new List<int>();
                    var efilList = new List<EvaluationFileInstanceLog>();
                    var evaluator = await _repo.User.GetUser(userId, true);
                    foreach (var goalCascadeDto in goalsCascadeDto)
                    {
                        if (goalCascadeDto.GoalForCreationDto.AxisInstanceId != 0)
                        {
                            evaluateeIds.Add(goalCascadeDto.EvaluateeId);
                            var evaluateeSheet = _repo.EvaluationFileInstance.GetEvaluationFileInstanceByUserId(goalCascadeDto.EvaluateeId, modelId).Result;
                            var efil = new EvaluationFileInstanceLog
                            {
                                Title = evaluateeSheet.Title,
                                Created = DateTime.Now,
                                Log = $"L'objectif: '{goalCascadeDto.GoalForCreationDto.Description}', a été ajouté à la fiche '{evaluateeSheet.Title}' par l'évaluateur {evaluator.FirstName} {evaluator.LastName}."
                            };
                            efilList.Add(efil);
                        }
                        else
                        {
                            var evaluatee = _repo.User.GetUser(goalCascadeDto.EvaluateeId, false).Result;
                            await SendNotificationsForEvaluator(goalCascadeDto.EvaluateeId,
                            $"Le sous-objectif: '{goalCascadeDto.GoalForCreationDto.Description}', n'a pas été cascadé au collaborateur {evaluatee.FirstName} {evaluatee.LastName}.");
                        }
                    }

                    await LogForSheet(efilList);

                    // Send Notifications
                    string emailContent = "Un nouvel objectif vous a été attribué par votre évaluateur.";
                    await SendNotificationsForSubordinate(userId, emailContent, evaluateeIds);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CascadeGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("createGoal")]
        public async Task<IActionResult> CreateGoal(int userId, GoalForCreationDto goalCreationDto)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                // Validate the total weights of the objectives within an axis instance
                var axisInstanceIds = new List<int> { goalCreationDto.AxisInstanceId };
                var goalsGroupedByAxisInstanceList = await GetAxisInstancesWithGoals(axisInstanceIds);
                if (IsTotalWeightOfObjectivesOver100(goalsGroupedByAxisInstanceList, goalCreationDto.Weight))
                {
                    return BadRequest("Le poids total de vos objectifs est supérieur à 100%!");
                }

                // Create a new goal
                var goal = _mapper.Map<Goal>(goalCreationDto);
                _repo.Goal.AddGoal(goal);
                await _repo.Goal.SaveAllAsync();

                return CreatedAtRoute("GetGoal", new { id = goal.Id }, goal);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("editGoal/{id}")]
        public async Task<IActionResult> UpdateGoal(int userId, int id, GoalForUpdateDto goalForUpdateDto)
        {
            try
            {
                var goalOwner = await _repo.Goal.GetGoalOwner(id);
                if (goalOwner == null) return NotFound();
                if (!await IsItAllowed(goalOwner.Id)) return Unauthorized();
                // Validate the total weights of the objectives within an axis instance
                var axisInstanceIds = new List<int> { goalForUpdateDto.AxisInstanceId };
                var goalsGroupedByAxisInstanceList = await GetAxisInstancesWithGoals(axisInstanceIds);
                if (IsTotalWeightOfObjectivesOver100(goalsGroupedByAxisInstanceList, goalForUpdateDto.Weight, id))
                {
                    return BadRequest("Le poids total de vos objectifs est supérieur à 100%!");
                }

                var goalFromRepo = await _repo.Goal.GetGoal(id);
                if (goalFromRepo == null) return BadRequest("La fiche d'évaluation n'existe pas!");

                //Prevent evaluator from update in case the sheet's status is in draft
                if (goalOwner.Id != userId && goalFromRepo.Status == Constants.DRAFT) return BadRequest("La fiche d'évaluation est encore en rédaction.");

                _mapper.Map(goalForUpdateDto, goalFromRepo);
                _repo.Goal.UpdateGoal(goalFromRepo);

                await _repo.Goal.SaveAllAsync();

                // Sent notification if deleted goal has parent
                if (goalFromRepo.ParentGoalId != 0)
                {
                    var parentGoalFromRepo = await _repo.Goal.GetGoal(goalFromRepo.ParentGoalId);
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    string emailContent = $"Le sous-objectif: '{goalFromRepo.Description}' de l'objectif '{parentGoalFromRepo.Description}', a été modifé dans la feuille d'évaluation de l'utilisateur {user.FirstName} {user.LastName}.";
                    await SendNotificationsForEvaluator(userId, emailContent);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("validateGoals")]
        public async Task<IActionResult> ValidateGoals(int userId, IEnumerable<GoalForUpdateDto> goalsToUpdateDto)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                // Get main data
                var goalIds = new List<int>();
                foreach (var goalToUpdateDto in goalsToUpdateDto)
                {
                    goalIds.Add(goalToUpdateDto.Id);
                }

                var goalsStatus = "";
                var emailContent = "";
                var sheetTitle = "";
                var sheetOwnerId = 0;
                foreach (var goalToUpdateDto in goalsToUpdateDto)
                {
                    goalsStatus = goalToUpdateDto.Status;
                    emailContent = goalToUpdateDto.EmailContent;
                    sheetTitle = goalToUpdateDto.SheetTitle;
                    sheetOwnerId = goalToUpdateDto.SheetOwnerId;
                    break;
                }

                // Validate the total weights of the objectives within an axis instance
                var axisInstanceIds = new List<int>();
                foreach (var goalToUpdateDto in goalsToUpdateDto)
                {
                    axisInstanceIds.Add(goalToUpdateDto.AxisInstanceId);
                }
                var goalsGroupedByAxisInstanceList = await GetAxisInstancesWithGoals(axisInstanceIds);
                foreach (var goalsGroupedByAxisInstance in goalsGroupedByAxisInstanceList)
                {
                    if (goalsGroupedByAxisInstance.TotalGoalWeight != 100 && Constants.DRAFT != goalsStatus)
                    {
                        return BadRequest("Le poids total des objectifs est différent de 100%!");
                    }
                }

                // Validate the total user weights
                var totalWeights = goalsGroupedByAxisInstanceList.Sum(a => a.UserWeight);
                if (totalWeights != 100)
                {
                    return BadRequest($"Pondération Utilisateur total est égale à {totalWeights}%, elle doit être égale à 100%.");
                }

                // Check if user has evaluator in the case of review
                if (Constants.REVIEW == goalsStatus && !await IsUserHasEvaluator(userId))
                {
                    return BadRequest("Vous n'avez pas d'évaluateur pour le moment!");
                }

                // Set Status of goals
                var goalsFromRepo = await _repo.Goal.GetGoalsByIds(goalIds);
                if (goalsFromRepo != null)
                {
                    foreach (var goal in goalsFromRepo)
                    {
                        goal.Status = goalsStatus;
                        _repo.Goal.UpdateGoal(goal);
                    }
                    await _repo.Goal.SaveAllAsync();

                    // Log objectifs have been submitted for validation
                    var efilList = new List<EvaluationFileInstanceLog>(){new EvaluationFileInstanceLog
                    {
                        Title = sheetTitle,
                        Created = DateTime.Now,
                        Log = $"Les objectives de la fiche: '{sheetTitle}' ont été mis au statut {goalsStatus}."
                    }};

                    await LogForSheet(efilList);

                    // Send Notification
                    if (goalsStatus == Constants.REVIEW)
                    {
                        await SendNotificationsForEvaluator(userId, emailContent);
                    }
                    else
                    {
                        await SendNotificationsForSubordinate(userId, emailContent, new List<int>() { sheetOwnerId });
                    }

                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ValidateGoals endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("deleteGoal/{id}")]
        public async Task<IActionResult> DeleteGoal(int userId, int id)
        {
            try
            {
                var goalOwner = await _repo.Goal.GetGoalOwner(id);
                if (goalOwner == null) return NotFound();
                if (!await IsItAllowed(goalOwner.Id)) return Unauthorized();

                var goalFromRepo = await _repo.Goal.GetGoal(id);
                if (goalFromRepo == null) return NotFound();

                // Prevent deleting the goal if it has children
                var children = await _repo.Goal.GetGoalChildren(goalFromRepo.Id);
                if (children != null && children.Count() > 0) return BadRequest("Cet objectif a des sous-objectifs.");

                //Prevent evaluator from deleting the goal in case the sheet's status is in draft except if he cascaded it
                if (goalOwner.Id != userId)
                {
                    if (goalFromRepo.ParentGoalId > 0)
                    {
                        var parentGoalOwner = await _repo.Goal.GetGoalOwner(goalFromRepo.ParentGoalId);
                        if (userId != parentGoalOwner.Id && goalFromRepo.Status == Constants.DRAFT) return BadRequest("La fiche d'évaluation est encore en rédaction.");
                    }
                    else
                    {
                        if (goalFromRepo.Status == Constants.DRAFT) return BadRequest("La fiche d'évaluation est encore en rédaction.");
                    }
                }

                // Proceed with delete
                _repo.Goal.DeleteGoal(goalFromRepo);
                await _repo.Goal.SaveAllAsync();

                // Sent notification if deleted goal has parent
                if (goalFromRepo.ParentGoalId != 0)
                {
                    var parentGoalFromRepo = await _repo.Goal.GetGoal(goalFromRepo.ParentGoalId);
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    string emailContent = $"Le sous-objectif: '{goalFromRepo.Description}' de l'objectif '{parentGoalFromRepo.Description}', a été supprimé de la feuille d'évaluation de l'utilisateur {user.FirstName} {user.LastName}.";
                    await SendNotificationsForEvaluator(userId, emailContent);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGoal endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task<List<AxisInstanceWithGoalsToReturnDto>> GetAxisInstancesWithGoals(IEnumerable<int> axisInstanceIds)
        {
            var goalsGroupedByAxisInstanceList = new List<AxisInstanceWithGoalsToReturnDto>();
            // Add Axis Instances
            var axisInstanceList = await _repo.AxisInstance.GetAxisInstancesByIds(axisInstanceIds);
            var ownerId = axisInstanceList.First().EvaluationFileInstance.OwnerId;
            var goalOwnerSelfEvaluator = await IsGoalOwnerSelfEvaluator(ownerId);
            foreach (var axisInstance in axisInstanceList)
            {
                var axisInstanceWithGoalsToReturnDto = new AxisInstanceWithGoalsToReturnDto()
                {
                    axisInstanceId = axisInstance.Id,
                    Title = axisInstance.Title,
                    Description = axisInstance.Description,
                    PoleName = axisInstance.PoleName,
                    PoleWeight = axisInstance.PoleWeight,
                    UserWeight = axisInstance.UserWeight
                };
                goalsGroupedByAxisInstanceList.Add(axisInstanceWithGoalsToReturnDto);
            }

            // Add goals for each axis instance
            var goalsFromRepo = await _repo.Goal.GetGoalsByAxisInstanceIds(axisInstanceIds);
            if (goalsFromRepo != null)
            {
                foreach (var goal in goalsFromRepo)
                {
                    var goalToReturn = await BuildGoalToReturn(goal);

                    if (goalsGroupedByAxisInstanceList.Exists(a => a.axisInstanceId == goal.AxisInstanceId))
                    {
                        goalsGroupedByAxisInstanceList.Find(a => a.axisInstanceId == goal.AxisInstanceId).Goals.Add(goalToReturn);
                    }
                }

                // Count and summup goals
                Decimal percentTotalGrade = 0.00m;
                foreach (var el in goalsGroupedByAxisInstanceList)
                {
                    el.TotalGoals = el.Goals.Count;
                    el.TotalGoalWeight = el.Goals.Count == 0 ? 0 : el.Goals.Sum(g => g.Weight);
                    el.GoalsStatus = el.Goals.Count == 0 ? Constants.NOTSTARTED : el.Goals.First().Status;

                    Decimal axisGrade = el.Goals.Select(g =>
                   {
                       var CompletionRate = GetCompletionRate(g, goalOwnerSelfEvaluator);
                       return g.Weight * el.UserWeight * CompletionRate;
                   }).Sum();
                    Decimal percentAxisGrade = axisGrade / 10000.00m;
                    percentTotalGrade += percentAxisGrade;
                    el.AxisGrade = percentAxisGrade.ToString("N2");
                    foreach (var goal in el.Goals)
                    {
                        var CompletionRate = GetCompletionRate(goal, goalOwnerSelfEvaluator);
                        Decimal percentGoalGrade = (goal.Weight * el.UserWeight * CompletionRate) / 10000.00m;
                        goal.GoalGrade = percentGoalGrade.ToString("N2");
                    }
                }

                foreach (var el in goalsGroupedByAxisInstanceList)
                {
                    el.TotalGrade = percentTotalGrade.ToString("N2");
                }
            }
            return goalsGroupedByAxisInstanceList;
        }

        private async Task<GoalToReturnDto> BuildGoalToReturn(Goal goal)
        {
            var goalToReturn = _mapper.Map<GoalToReturnDto>(goal);
            if (goal.ParentGoalId > 0)
            {
                var parentGoalOwner = await _repo.Goal.GetGoalOwner(goal.ParentGoalId);
                if (parentGoalOwner != null)
                {
                    goalToReturn.CascaderFullName = parentGoalOwner.FirstName + " " + parentGoalOwner.LastName;
                    var mainPhoto = parentGoalOwner.Photos.FirstOrDefault(p => p.IsMain);
                    if (mainPhoto != null)
                    {
                        goalToReturn.CascaderPhotoUrl = mainPhoto.Url;
                    }
                }
            }

            return goalToReturn;
        }

        private int GetCompletionRate(GoalToReturnDto goal, bool goalOwnerSelfEvaluator)
        {
            var lastGoalEvaluation = goal.GoalEvaluations.OrderByDescending(e => e.Created).FirstOrDefault();
            if (lastGoalEvaluation == null) return 0;
            else
            {
                if (lastGoalEvaluation.SelfEvaluation == false) return lastGoalEvaluation.CompletionRate;
                else
                {
                    if (goalOwnerSelfEvaluator) return lastGoalEvaluation.CompletionRate;
                }
                return 0;
            }
        }

        private bool IsTotalWeightOfObjectivesOver100(List<AxisInstanceWithGoalsToReturnDto> goalsGroupedByAxisInstanceList, int weight, int goalId = 0)
        {
            // Creation case
            if (goalId == 0)
            {
                if (goalsGroupedByAxisInstanceList[0].TotalGoalWeight + weight > 100)
                {
                    return true;
                }
            }
            else // update case
            {
                var totalWeight = weight;
                foreach (var goal in goalsGroupedByAxisInstanceList[0].Goals)
                {
                    if (goal.Id != goalId)
                    {
                        totalWeight = totalWeight + goal.Weight;
                    }
                }
                if (totalWeight > 100)
                {
                    return true;
                }
            }


            return false;
        }

        private async Task SendNotificationsForEvaluator(int userId, string emailContent)
        {
            var evaluators = await _repo.User.LoadEvaluators(userId);
            foreach (var evaluator in evaluators)
            {
                // Only first rank of evaluators
                if (evaluator.Rank == 1)
                {
                    var messageForCreationDto = new MessageForCreationDto()
                    {
                        RecipientId = evaluator.Id,
                        SenderId = userId,
                        Content = emailContent
                    };
                    var message = _mapper.Map<Message>(messageForCreationDto);
                    _repo.Message.AddMessage(message);
                }
            }
            await _repo.Message.SaveAllAsync();
        }
        private async Task SendNotificationsForSubordinate(int userId, string emailContent, List<int> sheetOwnerIds)
        {
            foreach (var sheetOwnerId in sheetOwnerIds)
            {
                var messageForCreationDto = new MessageForCreationDto()
                {
                    RecipientId = sheetOwnerId,
                    SenderId = userId,
                    Content = emailContent
                };
                var message = _mapper.Map<Message>(messageForCreationDto);
                _repo.Message.AddMessage(message);
            }

            await _repo.Message.SaveAllAsync();
        }

        private async Task LogForSheet(List<EvaluationFileInstanceLog> efilList)
        {
            foreach (var efil in efilList)
            {
                _repo.EvaluationFileInstanceLog.AddEvaluationFileInstanceLog(efil);
            }

            await _repo.EvaluationFileInstanceLog.SaveAllAsync();
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

        private async Task<bool> IsGoalOwnerSelfEvaluator(int goalOwnerId)
        {
            var evaluators = await _repo.User.LoadEvaluators(goalOwnerId);
            var evaluator = evaluators.FirstOrDefault(e => e.Id == goalOwnerId);
            if (evaluator == null) return false;
            return true;
        }
    }
}