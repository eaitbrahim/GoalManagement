using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Interfaces;
using SothemaGoalManagement.API.Models;

namespace SothemaGoalManagement.API.Controllers
{
    [Route("api/hr/[controller]")]
    [ApiController]
    public class AxisController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public AxisController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("addAxis")]
        public async Task<IActionResult> AddAxis(AxisForCreationDto axisForCreationDto)
        {
            try
            {
                var axis = _mapper.Map<Axis>(axisForCreationDto);

                _repo.Axis.AddAxis(axis);

                await _repo.Axis.SaveAllAsync();

                var axisToReturn = _mapper.Map<AxisToReturnDto>(axis);
                // map added axis to poles
                await mapNewAxisToPoles(axisToReturn.Id);
                return CreatedAtRoute("GetAxis", new { id = axis.Id }, axisToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddAxis enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task mapNewAxisToPoles(int axisId)
        {
            var poles = await _repo.Pole.GetPoles();
            foreach (var pole in poles)
            {
                var axisPole = new AxisPole()
                {
                    AxisId = axisId,
                    PoleId = pole.Id,
                    Weight = 0
                };
                _repo.AxisPole.AddAxisPole(axisPole);
            }

            await _repo.AxisPole.SaveAllAsync();
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("axis/{id}", Name = "GetAxis")]
        public async Task<IActionResult> GetAxis(int id)
        {
            try
            {
                var axisFromRepo = await _repo.Axis.GetAxis(id);

                if (axisFromRepo == null) return NotFound();

                var axisToReturn = _mapper.Map<AxisToReturnDto>(axisFromRepo);
                return Ok(axisToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAxis enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("axisList/{strategyId}")]
        public async Task<IActionResult> GetAxisList(int strategyId)
        {
            try
            {
                var axisListFromRepo = await _repo.Axis.GetAxisList(strategyId);

                var axisListToReturn = _mapper.Map<IEnumerable<AxisToReturnDto>>(axisListFromRepo);
                return Ok(axisListToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAxisList enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("updateAxis/{id}")]
        public async Task<IActionResult> UpdateAxis(int id, AxisForUpdateDto axisForUpdateDto)
        {
            try
            {
                var axisFromRepo = await _repo.Axis.GetAxis(id);
                if (axisFromRepo == null) return BadRequest("L'axe n'exist pas!");
                if (axisFromRepo.Sealed) return BadRequest("L'axe est scellé!");
                _mapper.Map(axisForUpdateDto, axisFromRepo);
                _repo.Axis.UpdateAxis(axisFromRepo);
                await _repo.Axis.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateAxis enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("axis/{id}/delete/{userId}")]
        public async Task<IActionResult> DeleteAxis(int id, int userId)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var axisFromRepo = await _repo.Axis.GetAxis(id);

                if (axisFromRepo == null) return NotFound();

                _repo.Axis.DeleteAxis(axisFromRepo);

                await _repo.Axis.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteAxis enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("axisPoleList/{axisId}")]
        public async Task<IActionResult> GetAxisPoleList(int axisId)
        {
            try
            {
                var axisListPoleFromRepo = await _repo.AxisPole.GetAxisPoleList(axisId);

                var axisPoleListToReturn = _mapper.Map<IEnumerable<AxisPoleToReturnDto>>(axisListPoleFromRepo);
                return Ok(axisPoleListToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAxisPoleList enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("updateAxisPole/{axisId}/{poleId}/{weight}")]
        public async Task<IActionResult> updateAxisPole(int axisId, int poleId, int weight)
        {
            try
            {
                var axisPoleFromRepo = await _repo.AxisPole.GetAxisPole(axisId, poleId);
                if (axisPoleFromRepo == null) return BadRequest("La pondération n'existe pas!");
                if (axisPoleFromRepo.Sealed) return BadRequest("La pondération est scellée!");

                axisPoleFromRepo.Weight = weight;
                _repo.AxisPole.UpdateAxisPole(axisPoleFromRepo);
                await _repo.AxisPole.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside updateAxisPole enfpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}