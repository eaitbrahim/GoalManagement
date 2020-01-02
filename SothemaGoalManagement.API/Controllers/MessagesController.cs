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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repo;
        private readonly IMapper _mapper;
        public MessagesController(ILoggerManager logger, IRepositoryWrapper repo, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var messageFromRepo = await _repo.Message.GetMessage(id);

                if (messageFromRepo == null) return NotFound();

                return Ok(messageFromRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMessage endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                messageParams.UserId = userId;
                var messagesFromRepo = await _repo.Message.GetMessagesForUser(messageParams);
                var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

                Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMessagesForUser endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var messageFromRepo = await _repo.Message.GetMessageThread(userId, recipientId);
                var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
                return Ok(messageThread);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMessageThread endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            try
            {
                var sender = await _repo.User.GetUser(userId, false);
                if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                messageForCreationDto.SenderId = userId;
                var recipient = await _repo.User.GetUser(messageForCreationDto.RecipientId, false);
                if (recipient == null) return BadRequest("Could not find user");
                var message = _mapper.Map<Message>(messageForCreationDto);

                _repo.Message.AddMessage(message);
                await _repo.Message.SaveAllAsync();
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMessage endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
                var messageFromRepo = await _repo.Message.GetMessage(id);

                if (messageFromRepo.SenderId == userId) messageFromRepo.SenderDeleted = true;

                if (messageFromRepo.RecipientId == userId) messageFromRepo.RecipientDeleted = true;

                if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                {
                    _repo.Message.DeleteMessage(messageFromRepo);
                    await _repo.Message.SaveAllAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMessage endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

                var message = await _repo.Message.GetMessage(id);
                if (message.RecipientId != userId) return Unauthorized();

                message.IsRead = true;
                message.DateRead = DateTime.Now;

                _repo.Message.UpdateMessage(message);
                await _repo.Message.SaveAllAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside MarkMessageAsRead endpoint: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}