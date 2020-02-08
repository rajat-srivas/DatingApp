using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister)
        {
            //validate request
            userForRegister.Username = userForRegister.Username.ToLower();
            if (await _repo.UserExists(userForRegister.Username))
                return BadRequest("Username already exists");

            var newUser = _mapper.Map<User>(userForRegister);

            var createdUser = await _repo.Register(newUser, userForRegister.Password);
            var userToReturn = _mapper.Map<UserForDetails>(createdUser);
            return CreatedAtRoute("GetUser",new { controller = "user", id = createdUser.Id}, userToReturn );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogin userforLogin)
        {
            var userForRepo = await _repo.Login(userforLogin.Username.ToLower(), userforLogin.Password);
            if (userForRepo == null) return Unauthorized();

            //Create a JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userForRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userForRepo.Username)
            };

            //hashed key for the token
            //key is stored in appsetting.json which is the secret for this app
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            //create the signing credential using the key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //create the token sescriptor which contains the token header and everything
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // use the token handler to create the token
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserForListDto>(userForRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token), user
            });

        }
    }
}