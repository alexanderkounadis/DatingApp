using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController (IAuthRepository repo, IConfiguration config) {
            _config = config;
            _repo = repo;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userForRegister) {
            // validate incoming request
            // we use apicontroller - no need for this
            // if(!ModelState.IsValid) return BadRequest(ModelState);
            userForRegister.Username = userForRegister.Username.ToLower ();
            if (await _repo.UserExists (userForRegister.Username)) return BadRequest ("Username already taken");

            User userToCreate = new User {
                Username = userForRegister.Username
            };

            User createdUser = await _repo.Register (userToCreate, userForRegister.Password);
            return StatusCode (201);
        }

        [HttpPost ("login")]
        public async Task<IActionResult> Login (UserForLoginDto user) {
            User userFromRepo = await _repo.Login (user.Username.ToLower(), user.Password);
            if (userFromRepo == null) return Unauthorized ();

            Claim[] claims = new Claim[] {
                new Claim (ClaimTypes.NameIdentifier, userFromRepo.Id.ToString ()),
                new Claim (ClaimTypes.Name, userFromRepo.Username)
            };
            // sign the token so the server could identify if this is a valid token
            SymmetricSecurityKey key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            // encrypt the signature
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims), 
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}