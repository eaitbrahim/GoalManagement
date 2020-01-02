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
    public class AxisPoleController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public AxisPoleController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
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
                _logger.LogError($"Something went wrong inside GetAxisPoleList endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
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
                _logger.LogError($"Something went wrong inside updateAxisPole endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}