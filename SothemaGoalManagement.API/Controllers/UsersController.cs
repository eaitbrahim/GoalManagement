using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SothemaGoalManagement.API.Interfaces;

namespace SothemaGoalManagement.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        public UsersController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userFromRepo = await _repo.User.GetUser(currentUserId, true);
                userParams.UserId = currentUserId;
                var users = await _repo.User.GetUsers(userParams);
                var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
                Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

                return Ok(usersToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUsers endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

                var user = await _repo.User.GetUser(id, isCurrentUser);

                var userToReturn = _mapper.Map<UserForDetailDto>(user);
                return Ok(userToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("loadAllUsers")]
        public async Task<IActionResult> loadAllUsers()
        {
            try
            {
                var userFromRepo = await _repo.User.LoadAllUsers();

                var userToReturn = _mapper.Map<IEnumerable<UserForSearchResultDto>>(userFromRepo);
                return Ok(userToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, ProfileForUpdateDto profileForUpdateDto)
        {
            try
            {
                if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var userFromRepo = await _repo.User.GetUser(id, true);
                _mapper.Map(profileForUpdateDto, userFromRepo);

                _repo.User.UpdateUser(userFromRepo);
                await _repo.User.SaveAllAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateProfile endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}