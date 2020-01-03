using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Controllers
{
    [Route("api/hr/[controller]")]
    [ApiController]
    public class EvaluationModelController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public EvaluationModelController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("{id}", Name = "GetEvaluationModel")]
        public async Task<IActionResult> GetEvaluationModel(int id)
        {
            try
            {
                var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFileDetail(id);

                if (evaluationFileFromRepo == null) return NotFound();

                return Ok(evaluationFileFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEvaluationModel endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("loadParameters/{evaluationModelId}")]
        public async Task<IActionResult> LoadParameters(int evaluationModelId)
        {
            try
            {
                var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFileDetail(evaluationModelId);

                if (evaluationFileFromRepo == null) return NotFound();
                var parametersToReturn = _mapper.Map<IEnumerable<ParametersToReturnDto>>(evaluationFileFromRepo.Parameters);
                return Ok(parametersToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside LoadParameters endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet]
        public async Task<IActionResult> GetEvaluationModelList([FromQuery]CommunParams evaluationFileParams)
        {
            try
            {
                var evaluationFilesWithBehaviorSkillsFromRepo = await _repo.EvaluationFile.GetEvaluationFiles(evaluationFileParams);

                return Ok(evaluationFilesWithBehaviorSkillsFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEvaluationModelList endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("new/{ownerId}")]
        public async Task<IActionResult> CreateEvaluationModel(int ownerId, EvaluationFileForCreationDto evaluationFileForCreationDto)
        {
            try
            {
                var user = await _repo.User.GetUser(ownerId, false);
                if (user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFileByStratgeyId(evaluationFileForCreationDto.StrategyId);
                if (evaluationFileFromRepo != null) return BadRequest("La stratégie sélectionnée est déjà utilisée par un autre modèle d'évaluation");

                evaluationFileForCreationDto.OwnerId = ownerId;

                var evaluationFile = _mapper.Map<EvaluationFile>(evaluationFileForCreationDto);

                _repo.EvaluationFile.AddEvaluationFile(evaluationFile);

                await _repo.EvaluationFile.SaveAllAsync();

                foreach (var bsId in evaluationFileForCreationDto.BehavioralSkillIds)
                {
                    var bsFromRepo = _repo.BehavioralSkill.GetBehavioralSkill(bsId).Result;
                    evaluationFile.BehavioralSkills.Add(new EvaluationFileBehavioralSkill { BehavioralSkill = bsFromRepo });
                    _repo.EvaluationFile.UpdateEvaluationFile(evaluationFile);
                }
                await _repo.EvaluationFile.SaveAllAsync();
                return CreatedAtRoute("GetEvaluationModel", new { id = evaluationFile.Id }, evaluationFile);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEvaluationModel endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("addParameters/{userId}")]
        public async Task<IActionResult> AddParameters(int userId, ParametersForCreationDto parametersForCreationDto)
        {
            try
            {
                var user = await _repo.User.GetUser(userId, false);
                if (user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var parameters = _mapper.Map<Parameters>(parametersForCreationDto);

                _repo.Parameters.AddParameters(parameters);

                await _repo.Parameters.SaveAllAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddParameters endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("{id}/delete/{ownerId}")]
        public async Task<IActionResult> DeleteEvaluationModel(int id, int ownerId)
        {
            try
            {
                var user = await _repo.User.GetUser(ownerId, false);
                if (user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFile(id);
                if (evaluationFileFromRepo == null) return NotFound();
                if (evaluationFileFromRepo.Status == Constants.PUBLISHED || evaluationFileFromRepo.Status == Constants.ARCHIVED)
                {
                    return BadRequest("Vous ne pouvez pas supprimer ce modele d\'evaluation car il est publié ou bien archivé.");
                }

                _repo.EvaluationFile.DeleteEvaluationFile(evaluationFileFromRepo);
                await _repo.EvaluationFile.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEvaluationModel endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("{id}/deleteParameters/{userId}")]
        public async Task<IActionResult> DeleteParameters(int id, int userId)
        {
            try
            {
                var user = await _repo.User.GetUser(userId, false);
                if (user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var parametersFromRepo = await _repo.Parameters.GetParameters(id);
                if (parametersFromRepo == null) return NotFound();

                _repo.Parameters.DeleteParameters(parametersFromRepo);
                await _repo.Parameters.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteParameters endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("edit/{ownerId}")]
        public async Task<IActionResult> UpdateEvaluationModel(int ownerId, EvaluationFileForUpdateDto evaluationFileForUpdateDto)
        {
            try
            {
                if (ownerId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFile(evaluationFileForUpdateDto.Id);
                if (evaluationFileFromRepo == null) return BadRequest("La fiche d'évaluation n'existe pas!");
                if (evaluationFileFromRepo.Sealed && evaluationFileForUpdateDto.Status != Constants.ARCHIVED) return BadRequest("La fiche d'évaluation est scellée!");
                if (evaluationFileFromRepo.Status != Constants.PUBLISHED && evaluationFileForUpdateDto.Status == Constants.PUBLISHED && evaluationFileFromRepo.Strategy.Sealed)
                {
                    return BadRequest("Vous ne pouvez pas publier cette fiche d'évaluation car une stratégie ou bien une compétence est déjà associée à une autre fiche d'évaluation.");
                }

                var publishEvaluation = false;
                var archiveEvaluation = false;
                if (evaluationFileFromRepo.Status != Constants.PUBLISHED && evaluationFileForUpdateDto.Status == Constants.PUBLISHED)
                {
                    publishEvaluation = true;
                }
                if (evaluationFileFromRepo.Status != Constants.ARCHIVED && evaluationFileForUpdateDto.Status == Constants.ARCHIVED)
                {
                    archiveEvaluation = true;
                }

                _mapper.Map(evaluationFileForUpdateDto, evaluationFileFromRepo);
                _repo.EvaluationFile.UpdateEvaluationFile(evaluationFileFromRepo);

                await _repo.EvaluationFile.SaveAllAsync();

                var efbsIdListFromRepo = await _repo.EvaluationFile.GetEvaluationFileBehavioralSkillIds(evaluationFileForUpdateDto.Id);
                var selectedBehavioralSkillIds = evaluationFileForUpdateDto.BehavioralSkillIds;
                selectedBehavioralSkillIds = selectedBehavioralSkillIds ?? new int[] { };
                foreach (var bsId in selectedBehavioralSkillIds.Except(efbsIdListFromRepo))
                {
                    evaluationFileFromRepo.BehavioralSkills.Add(new EvaluationFileBehavioralSkill { BehavioralSkillId = bsId, EvaluationFileId = evaluationFileFromRepo.Id });
                    _repo.EvaluationFile.UpdateEvaluationFile(evaluationFileFromRepo);
                }

                foreach (var bsId in efbsIdListFromRepo.Except(selectedBehavioralSkillIds))
                {
                    evaluationFileFromRepo.BehavioralSkills.Remove(new EvaluationFileBehavioralSkill { BehavioralSkillId = bsId, EvaluationFileId = evaluationFileFromRepo.Id });
                    _repo.EvaluationFile.UpdateEvaluationFile(evaluationFileFromRepo);
                }

                await _repo.EvaluationFile.SaveAllAsync();

                if (publishEvaluation)
                {
                    await PublishEvaluationModel(evaluationFileForUpdateDto.Id);
                }

                if (archiveEvaluation)
                {
                    await ArchiveEvaluationComponents(evaluationFileForUpdateDto.Id);
                }
                return Ok(evaluationFileFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateEvaluationModel endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task PublishEvaluationModel(int evaluationFileId)
        {
            var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFile(evaluationFileId);
            evaluationFileFromRepo.Sealed = true;
            evaluationFileFromRepo.SealedDate = DateTime.Now;
            var strategyFromRepo = await _repo.Strategy.GetStrategy(evaluationFileFromRepo.StrategyId);
            strategyFromRepo.Sealed = true;
            strategyFromRepo.SealedDate = DateTime.Now;
            foreach (var axis in strategyFromRepo.AxisList)
            {
                axis.Sealed = true;
                axis.SealedDate = DateTime.Now;
                foreach (var ap in axis.AxisPoles)
                {
                    ap.Sealed = true;
                    ap.SealedDate = DateTime.Now;
                }
            }

            var skillIds = await _repo.EvaluationFile.GetEvaluationFileBehavioralSkillIds(evaluationFileId);
            var behavioralSkillListFromRepo = await _repo.BehavioralSkill.GetBehavioralSkillsByIds(skillIds);
            foreach (var behavioralSkill in behavioralSkillListFromRepo)
            {
                behavioralSkill.Sealed = true;
                behavioralSkill.SealedDate = DateTime.Now;
                _repo.BehavioralSkill.UpdateBehavioralSkill(behavioralSkill);
            }

            await _repo.BehavioralSkill.SaveAllAsync();

        }
        private async Task ArchiveEvaluationComponents(int evaluationFileId)
        {
            var evaluationFileFromRepo = await _repo.EvaluationFile.GetEvaluationFile(evaluationFileId);
            var strategyFromRepo = await _repo.Strategy.GetStrategy(evaluationFileFromRepo.StrategyId);
            strategyFromRepo.Status = Constants.ARCHIVED;

            var skillIds = await _repo.EvaluationFile.GetEvaluationFileBehavioralSkillIds(evaluationFileId);
            var behavioralSkillListFromRepo = await _repo.BehavioralSkill.GetBehavioralSkillsByIds(skillIds);
            foreach (var behavioralSkill in behavioralSkillListFromRepo)
            {
                behavioralSkill.Status = Constants.ARCHIVED;
                _repo.BehavioralSkill.UpdateBehavioralSkill(behavioralSkill);
            }
            await _repo.BehavioralSkill.SaveAllAsync();
        }
    }
}