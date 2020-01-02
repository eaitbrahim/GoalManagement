using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Controllers
{
    [Route("api/hr/[controller]")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public StrategyController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("publishedStrategies")]
        public async Task<IActionResult> GetPublishedStrategies()
        {
            try
            {
                CommunParams strategyParams = new CommunParams()
                {
                    Status = Constants.PUBLISHED
                };

                var strategies = await _repo.Strategy.GetStrategies(strategyParams);

                var publishedStrategiesToReturnList = _mapper.Map<IEnumerable<Strategy>, IEnumerable<StrategyToReturnDto>>(strategies);


                return Ok(publishedStrategiesToReturnList);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPublishedStrategies endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("{id}", Name = "GetStrategy")]
        public async Task<IActionResult> GetStrategy(int ownerId, int id)
        {
            try
            {
                var strategyFromRepo = await _repo.Strategy.GetStrategy(id);

                if (strategyFromRepo == null) return NotFound();

                if (ownerId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) && strategyFromRepo.Status == Constants.DRAFT) return Unauthorized();

                var strategyToReturn = _mapper.Map<StrategyToReturnDto>(strategyFromRepo);
                return Ok(strategyToReturn);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet()]
        public async Task<IActionResult> GetStrategies([FromQuery]CommunParams strategyParams)
        {
            try
            {
                var strategies = await _repo.Strategy.GetStrategies(strategyParams);
                var strategiesToReturn = _mapper.Map<IEnumerable<StrategyForListDto>>(strategies);
                Response.AddPagination(strategies.CurrentPage, strategies.PageSize, strategies.TotalCount, strategies.TotalPages);

                return Ok(strategiesToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStrategies endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("new/{ownerId}")]
        public async Task<IActionResult> CreateStrategy(int ownerId, StrategyForCreationDto strategyForCreationDto)
        {
            try
            {
                var owner = await _repo.User.GetUser(ownerId, false);
                if (owner.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                strategyForCreationDto.OwnerId = ownerId;

                var strategy = _mapper.Map<Strategy>(strategyForCreationDto);

                _repo.Strategy.AddStrategy(strategy);

                await _repo.Strategy.SaveAllAsync();

                var strategyToReturn = _mapper.Map<StrategyToReturnDto>(strategy);
                return CreatedAtRoute("GetStrategy", new { id = strategy.Id }, strategyToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("clone/{ownerId}/{stratgeyId}")]
        public async Task<IActionResult> CloneStrategy(int ownerId, int stratgeyId)
        {
            try
            {
                var owner = await _repo.User.GetUser(ownerId, false);
                if (owner.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var strategyFromRepo = await _repo.Strategy.GetStrategy(stratgeyId);
                var newStratgey = new StrategyForCreationDto()
                {
                    Title = strategyFromRepo.Title + " - clone",
                    OwnerId = ownerId,
                    Description = strategyFromRepo.Description,
                };

                var strategy = _mapper.Map<Strategy>(newStratgey);

                _repo.Strategy.AddStrategy(strategy);

                await _repo.Strategy.SaveAllAsync();

                var axisListFromRepo = await _repo.Axis.GetAxisListDetailed(stratgeyId);

                foreach (var axis in axisListFromRepo)
                {
                    _repo.Axis.AddAxis(new Axis
                    {
                        Title = axis.Title,
                        Description = axis.Description,
                        StrategyId = strategy.Id
                    });
                }

                await _repo.Axis.SaveAllAsync();

                var newAxisListFromRepo = await _repo.Axis.GetAxisList(strategy.Id);
                foreach (var newAxis in newAxisListFromRepo)
                {
                    foreach (var axis in axisListFromRepo)
                    {
                        if (axis.Title == newAxis.Title)
                        {
                            foreach (var ap in axis.AxisPoles)
                            {
                                _repo.AxisPole.AddAxisPole(new AxisPole
                                {
                                    AxisId = newAxis.Id,
                                    PoleId = ap.PoleId,
                                    Weight = ap.Weight
                                });
                            }
                        }
                    }
                }
                await _repo.AxisPole.SaveAllAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CloneStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("delete/{strategyId}")]
        public async Task<IActionResult> DeleteStrategy(int strategyId)
        {
            try
            {
                var strategyFromRepo = await _repo.Strategy.GetStrategy(strategyId);

                if (strategyFromRepo == null) return NotFound();
                if (strategyFromRepo.Status == Constants.PUBLISHED || strategyFromRepo.Status == Constants.ARCHIVED) return BadRequest("Vous ne pouvez pas supprimer cette stratégie");

                _repo.Strategy.DeleteStrategy(strategyFromRepo);
                await _repo.Strategy.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("edit/{ownerId}")]
        public async Task<IActionResult> UpdateStrategy(int ownerId, StrategyForUpdateDto strategyForUpdateDto)
        {
            try
            {
                if (ownerId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var strategyFromRepo = await _repo.Strategy.GetStrategy(strategyForUpdateDto.Id);
                if (strategyFromRepo == null) return BadRequest("La stratégie n'existe pas!");
                if (strategyFromRepo.Sealed) return BadRequest("La stratégie est scellée!");
                if (strategyForUpdateDto.Status == Constants.PUBLISHED)
                {
                    if (strategyFromRepo.AxisList.Count == 0)
                    {
                        return BadRequest("Vous ne pouvez pas publier cette stratégie car elle n'a aucun axe.");
                    }

                    var weightsGroupedByPoles = await _repo.Axis.GetWeightsGroupedByPoles(strategyForUpdateDto.Id);
                    foreach (var axisPole in weightsGroupedByPoles)
                    {
                        if (axisPole.Weight != 100)
                        {
                            return BadRequest("Vous ne pouvez pas publier cette stratégie car une pondération de pôle n'est pas égale à 100%.");
                        }
                    }
                }

                _mapper.Map(strategyForUpdateDto, strategyFromRepo);
                _repo.Strategy.UpdateStrategy(strategyFromRepo);

                await _repo.Strategy.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("documentation/{id}")]
        public async Task<IActionResult> AddDocumentationForStrategy(int id, [FromForm]DocumentForStrategyDto documentForStrategyDto)
        {
            try
            {
                var strategyFromRepo = await _repo.Strategy.GetStrategy(id);

                if (strategyFromRepo.DocumentationUrl != null)
                {
                    return BadRequest("Vous ne pouvez télécharger qu'un seul document.");
                }

                var file = documentForStrategyDto.File;
                var uploadResult = new ImageUploadResult();
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                }

                strategyFromRepo.DocumentationUrl = uploadResult.Uri.ToString();
                strategyFromRepo.DocumentationPublicId = uploadResult.PublicId;

                _repo.Strategy.UpdateStrategy(strategyFromRepo);

                await _repo.Strategy.SaveAllAsync();
                var stratgeyToReturn = _mapper.Map<StrategyToReturnDto>(strategyFromRepo);
                return CreatedAtRoute("GetStrategy", new { id = strategyFromRepo.Id }, stratgeyToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddDocumentationForStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("documentation/delete/{id}")]
        public async Task<IActionResult> DeleteDocumentationForStrategy(int id)
        {
            try
            {
                var strategyFromRepo = await _repo.Strategy.GetStrategy(id);

                if (strategyFromRepo.DocumentationPublicId != null)
                {
                    var deleteParams = new DeletionParams(strategyFromRepo.DocumentationPublicId);
                    var result = _cloudinary.Destroy(deleteParams);
                    if (result.Result == "ok")
                    {
                        strategyFromRepo.DocumentationUrl = null;
                        strategyFromRepo.DocumentationPublicId = null;
                        _repo.Strategy.UpdateStrategy(strategyFromRepo);

                        await _repo.Strategy.SaveAllAsync();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteDocumentationForStrategy endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}