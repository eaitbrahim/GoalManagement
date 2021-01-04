using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using SothemaGoalManagement.API.Interfaces;
using System.Security.Claims;

namespace SothemaGoalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;

        private readonly IConfiguration _config;

        public AdminController(IConfiguration config, DataContext context, IRepositoryWrapper repo, IMapper mapper, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _config = config;
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _userManager = userManager;
            _context = context;

            Account acc = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);

        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("emailAlreadyExists")]
        public async Task<IActionResult> EmailAlreadyExists(string email)
        {
            try
            {
                var result = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);

                return Ok(result != null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EmailAlreadyExists endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("employeeNumberAlreadyExists")]
        public async Task<IActionResult> EmployeeNumberAlreadyExists(string employeeNumber)
        {
            try
            {
                var result = await _repo.User.EmployeeNumberAlreadyExists(employeeNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EmployeeNumberAlreadyExists endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpPost("register/{notifyUser}")]
        public async Task<IActionResult> Register(bool notifyUser, [FromBody]UserForRegisterDto userForRegisterDto)
        {
            try
            {
                // Make sure employee number and emails are unique
                var checkEmployeeNumber = await _repo.User.EmployeeNumberAlreadyExists(userForRegisterDto.EmployeeNumber);
                if (checkEmployeeNumber) return BadRequest("Matricule existe déjà.");

                var checkEmail = await _userManager.FindByNameAsync(userForRegisterDto.Email) ?? await _userManager.FindByEmailAsync(userForRegisterDto.Email);
                if (checkEmail != null) return BadRequest("Email existe déjà");

                var userToCreate = _mapper.Map<User>(userForRegisterDto);
                userToCreate.UserName = userForRegisterDto.Email;
                userToCreate.EmployeeNumber = userForRegisterDto.EmployeeNumber.ToLower();
                IdentityResult result = null;
                result = _userManager.CreateAsync(userToCreate, "Password123").Result;
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userToCreate, "Collaborator");
                    var userToReturn = _mapper.Map<UserForDetailDto>(userToCreate);

                    if (result.Succeeded)
                    {
                        if (notifyUser)
                        {
                            // Notify user by email to change his default password
                            var generatedToken = await _userManager.GeneratePasswordResetTokenAsync(userToCreate);
                            generatedToken = System.Web.HttpUtility.UrlEncode(generatedToken);
                            var content = $"{userToCreate.FirstName},<br><p>Votre nouveau compte pour l'application Goal management a été créé.</p><p></p><p>Veuillez réinitialiser votre mot de passe, en suivant ce lien(ou bien collez - le dans votre navigateur) dans les 90 prochaines minutes:<br>{this.Request.Scheme}://{this.Request.Host}/resetPassword?token={generatedToken}&email={userToCreate.Email}</p>";
                            await new Mailer(_config).SendEmail(userToCreate.Email, generatedToken, "Bienvenue à l'application Goal Management", content);
                        }
                        return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
                    }

                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Register endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles([FromQuery]UserParams userParams)
        {
            try
            {
                var users = await _repo.User.GetUsersWithRoles(userParams);
                Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUsersWithRoles endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departmentList = await _repo.Department.GetDepartments();
                var departmentsToReturn = _mapper.Map<IEnumerable<DepartmentToReturnDto>>(departmentList);

                return Ok(departmentsToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDepartments endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("poles")]
        public async Task<IActionResult> GetPoless()
        {
            try
            {
                var poleList = await _repo.Pole.GetPoles();
                var polesToReturn = _mapper.Map<IEnumerable<PoleToReturnDto>>(poleList);

                return Ok(polesToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPoless endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("userStatus")]
        public async Task<IActionResult> GetUserStatus()
        {
            try
            {
                var userStatusList = await _repo.UserStatus.GetUserStatus();
                var userStatusToReturn = _mapper.Map<IEnumerable<UserStatusToReturnDto>>(userStatusList);

                return Ok(userStatusToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUserStatus endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpPost("editRoles/{userId}")]
        public async Task<IActionResult> EditRoles(string userId, RoleEditDto roleEditDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var userRoles = await _userManager.GetRolesAsync(user);

                var selectedRoles = roleEditDto.RoleNames;
                selectedRoles = selectedRoles ?? new string[] { };
                var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

                if (!result.Succeeded) return BadRequest("Failed to add to roles");

                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

                if (!result.Succeeded) return BadRequest("Failed to remove the roles");

                return Ok(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EditRoles endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            try
            {
                var photos = await _context.Photos.Include(u => u.User).IgnoreQueryFilters().Where(p => p.IsApproved == false).Select(u => new
                {
                    Id = u.Id,
                    UserName = u.User.UserName,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).ToListAsync();

                return Ok(photos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPhotosForModeration endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            try
            {
                var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);
                photo.IsApproved = true;
                await _context.SaveChangesAsync();

                // Send Notifications
                string emailContent = "Votre photo a été approuvée.";
                await SendNotificationToUser(photo.UserId, emailContent);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ApprovePhoto endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminHRRoles")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            try
            {
                var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);

                if (photo.IsMain) return BadRequest("You cannot reject the main photo");

                if (photo.PublicId != null)
                {
                    var deleteParams = new DeletionParams(photo.PublicId);
                    var result = _cloudinary.Destroy(deleteParams);
                    if (result.Result == "ok") _context.Photos.Remove(photo);
                }

                var userId = photo.UserId;
                if (photo.PublicId == null) _context.Photos.Remove(photo);

                await _context.SaveChangesAsync();

                // Send Notifications
                string emailContent = "Votre photo a été refusée.";
                await SendNotificationToUser(userId, emailContent);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside RejectPhoto endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateUser(UserForUpdateDto userForUpdateDto)
        {
            try
            {
                // Double check duplicate employee numbers
                var checkEmployeeNumber = await _repo.User.EmployeeNumberAlreadyExists(userForUpdateDto.EmployeeNumber, userForUpdateDto.Id);
                if (checkEmployeeNumber) return BadRequest("Matricule existe déjà.");

                // double check dupkicate emails
                var userFoundWithEmail = await _userManager.FindByNameAsync(userForUpdateDto.Email) ?? await _userManager.FindByEmailAsync(userForUpdateDto.Email);
                if (userFoundWithEmail != null && userFoundWithEmail.Id != userForUpdateDto.Id)
                {
                    return BadRequest("Email existe déjà.");
                }

                // proceed with update
                var userFromRepo = await _repo.User.GetUser(userForUpdateDto.Id, true);
                userFromRepo.UserName = userForUpdateDto.Email;
                _mapper.Map(userForUpdateDto, userFromRepo);
                _repo.User.UpdateUser(userFromRepo);
                await _repo.User.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpGet("searchUsers")]
        public async Task<IActionResult> searchUsers([FromQuery]UserParams searchParams)
        {
            try
            {
                var usersFromRepo = await _repo.User.SerachForUsers(searchParams.UserToSearch, searchParams.UserStatusId);

                var usersToReturn = _mapper.Map<IEnumerable<UserForSearchResultDto>>(usersFromRepo);
                return Ok(usersToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside searchUsers endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("loadEvaluators/{evaluatedId}")]
        public async Task<IActionResult> LoadEvaluators(int evaluatedId)
        {
            try
            {
                var evaluatorsFromRepo = await _repo.User.LoadEvaluators(evaluatedId);
                return Ok(evaluatorsFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside LoadEvaluators endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("loadEvaluatees/{evaluatorId}")]
        public async Task<IActionResult> LoadEvaluatees(int evaluatorId)
        {
            try
            {
                var evaluateesFromRepo = await _repo.User.LoadEvaluatees(evaluatorId);
                return Ok(evaluateesFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside LoadEvaluatees endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("addEvaluatorToUser/{evaluatedId}")]
        public async Task<IActionResult> AddEvaluatorToUser(int evaluatedId, IEnumerable<int> evaluatorIds)
        {
            try
            {
                foreach (var evaluatorId in evaluatorIds)
                {
                    var evaluatedEvaluator = new EvaluatedEvaluator()
                    {
                        EvaluatedId = evaluatedId,
                        EvaluatorId = evaluatorId,
                        Rank = 1
                    };

                    var evaluatedEvaluatorFromRepo = _context.EvaluatedEvaluators.SingleOrDefaultAsync(ee => ee.EvaluatedId == evaluatedId && ee.EvaluatorId == evaluatorId).Result;

                    if (evaluatedEvaluatorFromRepo == null)
                    {
                        _context.EvaluatedEvaluators.Add(evaluatedEvaluator);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddEvaluatorToUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPost("addEvaluateeToUser/{evaluatorId}")]
        public async Task<IActionResult> AddEvaluateeToUser(int evaluatorId, IEnumerable<int> evaluateeIds)
        {
            try
            {
                var evaluatorFromRepo = await _repo.User.GetUser(evaluatorId, false);
                foreach (var evaluateeId in evaluateeIds)
                {
                    var evaluatedEvaluator = new EvaluatedEvaluator()
                    {
                        EvaluatedId = evaluateeId,
                        EvaluatorId = evaluatorId,
                        Rank = 1
                    };
                    var evaluatedEvaluatorFromRepo = _context.EvaluatedEvaluators.SingleOrDefaultAsync(ee => ee.EvaluatedId == evaluateeId && ee.EvaluatorId == evaluatorId).Result;

                    if (evaluatedEvaluatorFromRepo == null)
                    {
                        _context.EvaluatedEvaluators.Add(evaluatedEvaluator);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddEvaluateeToUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpPut("updateRankOfEvaluator/{evaluatedId}/{evaluatorId}/{rank}")]
        public async Task<IActionResult> UpdateRankOfEvaluator(int evaluatedId, int evaluatorId, int rank)
        {
            try
            {
                var evaluatedEvaluatorFromRepo = await _context.EvaluatedEvaluators.SingleOrDefaultAsync(ee => ee.EvaluatedId == evaluatedId && ee.EvaluatorId == evaluatorId);
                if (evaluatedEvaluatorFromRepo == null) return NotFound();
                evaluatedEvaluatorFromRepo.Rank = rank;
                _context.EvaluatedEvaluators.Update(evaluatedEvaluatorFromRepo);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRankOfEvaluator endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("deleteEvaluator/{evaluatorId}/{evaluatedId}")]
        public async Task<IActionResult> DeleteEvaluator(int evaluatorId, int evaluatedId)
        {
            try
            {
                var evaluatedEvaluatorFromRepo = await _context.EvaluatedEvaluators.SingleOrDefaultAsync(ee => ee.EvaluatedId == evaluatedId && ee.EvaluatorId == evaluatorId);

                if (evaluatedEvaluatorFromRepo == null) return NotFound();

                _context.EvaluatedEvaluators.Remove(evaluatedEvaluatorFromRepo);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEvaluatorEvaluatee endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "RequireHRHRDRoles")]
        [HttpDelete("deleteEvaluatee/{evaluatedId}/{evaluatorId}")]
        public async Task<IActionResult> DeleteEvaluatorEvaluatee(int evaluatedId, int evaluatorId)
        {
            try
            {
                var evaluatedEvaluatorFromRepo = await _context.EvaluatedEvaluators.SingleOrDefaultAsync(ee => ee.EvaluatedId == evaluatedId && ee.EvaluatorId == evaluatorId);

                if (evaluatedEvaluatorFromRepo == null) return NotFound();

                _context.EvaluatedEvaluators.Remove(evaluatedEvaluatorFromRepo);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteEvaluatorEvaluatee endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task SendNotificationToUser(int userId, string emailContent)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Constants.ADMIN);
            foreach (var admin in admins)
            {
                var messageForCreationDto = new MessageForCreationDto()
                {
                    RecipientId = userId,
                    SenderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    Content = emailContent
                };
                var message = _mapper.Map<Message>(messageForCreationDto);
                _repo.Message.AddMessage(message);
            }

            await _repo.Message.SaveAllAsync();
        }
    }
}