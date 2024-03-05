using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthentication.Dtos;
using UserAuthentication.Models;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            
        }


        //Signup for Account
        [HttpPost("sign-up")]

        public async Task<IActionResult> SignUp(RegisterDto registerdto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User()
            {
                FullName = registerdto.UserName,
                Email = registerdto.Email,
                UserName = registerdto.UserName
            };
            var result = await _userManager.CreateAsync(user, registerdto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if(registerdto.Roles is null)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                foreach(var role in registerdto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            return Ok(new AuthResponseDto
            {
                Message = "User Created Successfully",
                IsSuccess = true
            });
        }



        //Login for Account
        [HttpPost("login")]

        public async Task<IActionResult> SignIn(LoginDto logindto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var User = await _userManager.FindByEmailAsync(logindto.Email);

            if(User is null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Message = "Invalid Email or No account exists",
                    IsSuccess = false
                });
            }

            var result = await _userManager.CheckPasswordAsync(User, logindto.Password);
            

            if(!result)
            {
                return Unauthorized(new AuthResponseDto { 
                
                    Message = "Invalid Password",
                    IsSuccess = false
                });
            }

            var token = GenerateJwtToken(User);

            return Ok(new AuthResponseDto
            {
                Token = token,
                IsSuccess = true,
                Message = "Login Successful"
            });

        }
        //token generate for login

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWTSetting").GetSection("Secret").Value!);
            
            var roles = _userManager.GetRolesAsync(user).Result;

            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email??""),
                new Claim(JwtRegisteredClaimNames.Name, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JWTSetting").GetSection("Audience").Value!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JWTSetting").GetSection("Issuer").Value!)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);

        }
    }
}
