using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Authentication server.  Just authenticates admin user for now
/// </summary>
namespace WebApplication1.Controllers
{
    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration config;

        public AuthController(IConfiguration  config)
        {
            this.config = config;
        }

        // POST: api/Auth
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] LoginDTO user)
        {
            /* Cheat, don't bother with DB and just have a fixed
             * admin user */
             if (user.Username == config["AdminUser:username"] &&
                user.Password == config["AdminUser:password"])
            {
                var jwt = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, "root"),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(config["Tokens:Issuer"], 
                    config["Tokens:Issuer"], jwt, expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return Ok(new { bearer = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest("Invalid user/password");
        }
    }
}
