using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SothemaGoalManagement.API.Data;
using SothemaGoalManagement.API.Dtos;
using SothemaGoalManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SothemaGoalManagement.API.Helpers;
using SothemaGoalManagement.API.Interfaces;

namespace SothemaGoalManagement.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(ILoggerManager logger, IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
                var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users.Include(p => p.Photos)
                                                          .Include(d => d.Department)
                                                          .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                    var userToReturn = _mapper.Map<UserForDetailDto>(appUser);
                    return Ok(new
                    {
                        token = GenerateJwtToken(appUser).Result,
                        user = userToReturn
                    });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Login endpoint: {ex.StackTrace}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(UserForResetPasswordDto userForResetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userForResetPasswordDto.Email);
                if (user == null)
                {
                    return BadRequest("Email n'existe plus dans l'application!");
                }
                try
                {
                    var result = await _userManager.ResetPasswordAsync(user, userForResetPasswordDto.Token, userForResetPasswordDto.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    return BadRequest(result.Errors);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ResetPassword endpoint: {ex.StackTrace}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("generatePasswordResetToken")]
        public async Task<IActionResult> GeneratePasswordResetToken(UserForResetPasswordDto userForResetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userForResetPasswordDto.Email);
                if (user == null)
                {
                    return BadRequest("Erreur lors de la réinitialisation de votre mot de passe!");
                }

                var generatedToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                generatedToken = System.Web.HttpUtility.UrlEncode(generatedToken);

                var content = $"{user.FirstName},<br><p>Quelqu'un a demandé de réinitialiser le mot de passe de votre compte.</p><p>Si vous n'avez pas demandé de réinitialisation de mot de passe, vous pouvez ignorer cet email.</p><p>Aucune modification n'a été apportée à votre compte.</p><p>Pour réinitialiser votre mot de passe, suivez ce lien(ou collez - le dans votre navigateur) dans les 90 prochaines minutes:<br>{this.Request.Scheme}://{this.Request.Host}/resetPassword?token={generatedToken}&email={user.Email}</p>";
                await new Mailer(_config).SendEmail(user.Email, generatedToken, "Réinitialisation de votre mot de passe Goal Management", content);
                return Ok(new { message = "Le lien de réinitialisation du mot de passe a été envoyé à votre adresse e-mail!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GeneratePasswordResetToken endpoint: {ex.StackTrace}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}