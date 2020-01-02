using System;
using System.Collections.Generic;
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
    public class BehavioralSkillController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public BehavioralSkillController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetBehavioralSkill")]
        public async Task<IActionResult> GetBehavioralSkill(int id)
        {
            try
            {
                var behavioralSkillFromRepo = await _repo.BehavioralSkill.GetBehavioralSkill(id);

                if (behavioralSkillFromRepo == null) return NotFound();

                return Ok(behavioralSkillFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBehavioralSkill endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBehavioralSkillList([FromQuery]CommunParams behavioralSkillParams)
        {
            try
            {
                var behavioralSkillsFromRepo = await _repo.BehavioralSkill.GetBehavioralSkills(behavioralSkillParams);
                var behavioralSkillsToReturn = _mapper.Map<IEnumerable<BehavioralSkillToReturnDto>>(behavioralSkillsFromRepo);
                return Ok(behavioralSkillsToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBehavioralSkillList endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("new/{createdById}")]
        public async Task<IActionResult> CreateBehavioralSkill(int createdById, BehavioralSkillForCreationDto behavioralSkillForCreationDto)
        {
            try
            {
                var user = await _repo.User.GetUser(createdById, false);
                if (user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                behavioralSkillForCreationDto.CreatedById = createdById;

                var behavioralSkill = _mapper.Map<BehavioralSkill>(behavioralSkillForCreationDto);

                _repo.BehavioralSkill.AddBehavioralSkill(behavioralSkill);
                await _repo.BehavioralSkill.SaveAllAsync();

                var behavioralSkillToReturn = _mapper.Map<BehavioralSkillToReturnDto>(behavioralSkill);
                return CreatedAtRoute("GetBehavioralSkill", new { id = behavioralSkill.Id }, behavioralSkillToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBehavioralSkill endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("clone/{ownerId}/{BehavioralSkillId}")]
        public async Task<IActionResult> CloneBehavioralSkill(int ownerId, int BehavioralSkillId)
        {
            try
            {
                var owner = await _repo.User.GetUser(ownerId, false);
                if (owner.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var behavioralSkillFromRepo = await _repo.BehavioralSkill.GetBehavioralSkill(BehavioralSkillId);
                var newBehavioralSkill = new BehavioralSkillForCreationDto()
                {
                    Skill = behavioralSkillFromRepo.Skill + " - clone",
                    CreatedById = ownerId,
                    Definition = behavioralSkillFromRepo.Definition,
                    LevelOne = behavioralSkillFromRepo.LevelOne,
                    LevelOneGrade = behavioralSkillFromRepo.LevelOneGrade,
                    LevelOneDescription = behavioralSkillFromRepo.LevelOneDescription,
                    LevelTwo = behavioralSkillFromRepo.LevelTwo,
                    LevelTwoGrade = behavioralSkillFromRepo.LevelTwoGrade,
                    LevelTwoDescription = behavioralSkillFromRepo.LevelTwoDescription,
                    LevelThree = behavioralSkillFromRepo.LevelThree,
                    LevelThreeGrade = behavioralSkillFromRepo.LevelThreeGrade,
                    LevelThreeDescription = behavioralSkillFromRepo.LevelThreeDescription,
                    LevelFour = behavioralSkillFromRepo.LevelFour,
                    LevelFourGrade = behavioralSkillFromRepo.LevelFourGrade,
                    LevelFourDescription = behavioralSkillFromRepo.LevelFourDescription,
                };

                var behavioralSkill = _mapper.Map<BehavioralSkill>(newBehavioralSkill);

                _repo.BehavioralSkill.AddBehavioralSkill(behavioralSkill);

                await _repo.BehavioralSkill.SaveAllAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CloneBehavioralSkill endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("delete/{BehavioralSkillId}")]
        public async Task<IActionResult> DeleteStrategy(int BehavioralSkillId)
        {
            try
            {
                var behavioralSkillFromRepo = await _repo.BehavioralSkill.GetBehavioralSkill(BehavioralSkillId);

                if (behavioralSkillFromRepo == null) return NotFound();
                if (behavioralSkillFromRepo.Status == Constants.PUBLISHED || behavioralSkillFromRepo.Status == Constants.ARCHIVED) return BadRequest("Vous ne pouvez pas supprimer cette stratégie");

                _repo.BehavioralSkill.DeleteBehavioralSkill(behavioralSkillFromRepo);
                await _repo.BehavioralSkill.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("edit/{createdById}")]
        public async Task<IActionResult> UpdateBehavioralSkill(int createdById, BehavioralSkillForUpdateDto behavioralSkillForUpdateDto)
        {
            try
            {
                if (createdById != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var behavioralSkillFromRepo = await _repo.BehavioralSkill.GetBehavioralSkill(behavioralSkillForUpdateDto.Id);
                if (behavioralSkillFromRepo.Sealed) return BadRequest("La compétence comportementale est scellée!");

                _mapper.Map(behavioralSkillForUpdateDto, behavioralSkillFromRepo);
                _repo.BehavioralSkill.UpdateBehavioralSkill(behavioralSkillFromRepo);
                await _repo.BehavioralSkill.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBehavioralSkill endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("publishedBehavioralSkills")]
        public async Task<IActionResult> GetPublishedBehavioralSkills()
        {
            try
            {
                CommunParams communParams = new CommunParams()
                {
                    Status = Constants.PUBLISHED
                };

                var behavioralSkills = await _repo.BehavioralSkill.GetBehavioralSkills(communParams);

                var publishedBehavioralSkillsToReturnList = _mapper.Map<IEnumerable<BehavioralSkill>, IEnumerable<BehavioralSkillToReturnDto>>(behavioralSkills);

                return Ok(publishedBehavioralSkillsToReturnList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPublishedBehavioralSkills endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}