using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase {
        private readonly IAuthRepository _repo;
        public AuthController (IAuthRepository repo) {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister){
            // validate incoming request
            // we use apicontroller - no need for this
            // if(!ModelState.IsValid) return BadRequest(ModelState);
            userForRegister.Username = userForRegister.Username.ToLower();
            if(await _repo.UserExists(userForRegister.Username)) return BadRequest("Username already taken");

            User userToCreate = new User{
                Username = userForRegister.Username
            };

            User createdUser = await _repo.Register(userToCreate, userForRegister.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto user){
            User userFromRepo = await _repo.Login(user.Username, user.Password);
            if(userFromRepo == null) return Unauthorized();
            return null;
        }

    }
}