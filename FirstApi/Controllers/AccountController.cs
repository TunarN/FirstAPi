using FirstApi.Dtos.User;
using FirstApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _iconfiguration;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration iconfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _iconfiguration = iconfiguration;
        }

        [HttpGet("role")]
        public async Task<IActionResult> CreateRole()
        {
            var result = await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            result = await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            result = await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
            return StatusCode(201);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var user = await _userManager.FindByNameAsync(registerDto.UserName);
            if (user != null) return StatusCode(409);
            user = new AppUser()
            {
                UserName = registerDto.UserName,
                FullName = registerDto.FullName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded) return BadRequest(result.Errors);

            return StatusCode(201);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null) return NotFound();
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) return NotFound();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_iconfiguration["JWT:SecretKey"]);

            var claimList = new List<Claim>
            {
              new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.NameIdentifier, user.Id)

            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                claimList.Add(new Claim("role", item));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _iconfiguration["JWT:Audience"],
                Issuer = _iconfiguration["JWT:Issuer"],
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });

        }
    }
}
