using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SothemaGoalManagement.API.Interfaces;
using System;
using Microsoft.AspNetCore.Identity;

namespace SothemaGoalManagement.API.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private ILoggerManager _logger;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private IRepositoryWrapper _repo;
        private readonly UserManager<User> _userManager;
        private Cloudinary _cloudinary;
        public PhotosController(ILoggerManager logger, UserManager<User> userManager, IRepositoryWrapper repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _logger = logger;
            _repo = repo;
            _userManager = userManager;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            try
            {
                var photoFromRepo = await _repo.Photo.GetPhoto(id);
                var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
                return Ok(photo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPhoto endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var userFromRepo = await _repo.User.GetUser(userId, true);

                // Users can have only 2 photos
                if (userFromRepo.Photos.Count >= 2)
                {
                    return BadRequest("Vous ne pouvez télécharger que 2 photos.");
                }

                var file = photoForCreationDto.File;
                var uploadResult = new ImageUploadResult();
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                }

                photoForCreationDto.Url = uploadResult.Uri.ToString();
                photoForCreationDto.PublicId = uploadResult.PublicId;

                var photo = _mapper.Map<Photo>(photoForCreationDto);
                if (!userFromRepo.Photos.Any(u => u.IsMain)) photo.IsMain = true;

                userFromRepo.Photos.Add(photo);
                _repo.User.UpdateUser(userFromRepo);

                await _repo.User.SaveAllAsync();

                // Send Notifications
                string emailContent = $"Une nouvelle photo de {userFromRepo.FirstName} {userFromRepo.LastName} a été soumise pour approbation.";
                await SendNotificationsForAdmins(userFromRepo.Id, emailContent);

                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AddPhotoForUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var userFromRepo = await _repo.User.GetUser(userId, true);

                if (!userFromRepo.Photos.Any(p => p.Id == id)) return Unauthorized();

                var photoFromRepo = await _repo.Photo.GetPhoto(id);
                if (photoFromRepo.IsMain) return BadRequest("This is already the main photo");

                var currentMainPhoto = await _repo.Photo.GetMainPhotoForUser(userId);
                currentMainPhoto.IsMain = false;
                photoFromRepo.IsMain = true;
                _repo.Photo.UpdatePhoto(currentMainPhoto);
                _repo.Photo.UpdatePhoto(photoFromRepo);

                await _repo.Photo.SaveAllAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SetMainPhoto endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeltePhoto(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var userFromRepo = await _repo.User.GetUser(userId, true);

                if (!userFromRepo.Photos.Any(p => p.Id == id)) return Unauthorized();

                var photoFromRepo = await _repo.Photo.GetPhoto(id);
                if (photoFromRepo.IsMain) return BadRequest("You cannot delete your main photo");

                if (photoFromRepo.PublicId != null)
                {
                    var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                    var result = _cloudinary.Destroy(deleteParams);
                    if (result.Result == "ok")
                    {
                        _repo.Photo.DeletePhoto(photoFromRepo);
                    }
                }

                if (photoFromRepo.PublicId == null)
                {
                    _repo.Photo.DeletePhoto(photoFromRepo);
                }

                await _repo.Photo.SaveAllAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeltePhoto endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task SendNotificationsForAdmins(int userId, string emailContent)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Constants.ADMIN);
            foreach (var admin in admins)
            {
                var messageForCreationDto = new MessageForCreationDto()
                {
                    RecipientId = admin.Id,
                    SenderId = userId,
                    Content = emailContent
                };
                var message = _mapper.Map<Message>(messageForCreationDto);
                _repo.Message.AddMessage(message);
            }

            await _repo.Message.SaveAllAsync();
        }
    }
}