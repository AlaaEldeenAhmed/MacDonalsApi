using AuthenticationPlugin;
using MacDonalsApi.Data;
using MacDonalsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MacDonalsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthService _auth;

        public AccountsController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var UserWithSamEmail = _context.users.SingleOrDefault(x => x.Email == user.Email);
            if (UserWithSamEmail != null)
            {
                return BadRequest("User With This Email Already Exists");
            }
            else
            {
                var UserObject = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = SecurePasswordHasherHelper.Hash(user.Password),
                    Role = "User"
                };

                _context.users.Add(UserObject);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] User user)
        {
            var UserEmail = _context.users.SingleOrDefault(x => x.Email == user.Email);
            if (UserEmail == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                var HashedPassword = UserEmail.Password;
                if (!SecurePasswordHasherHelper.Verify(user.Password, HashedPassword))
                {
                    return Unauthorized("This is User is Not Authorize");
                }
                else
                {
                    var claims = new[] {

                             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                             new Claim(JwtRegisteredClaimNames.Email, user.Email),
                             new Claim(ClaimTypes.Name, user.Email),
                             new Claim(ClaimTypes.Role, UserEmail.Role)
                    };

                    var token = _auth.GenerateAccessToken(claims);

                    return new ObjectResult(new
                    {
                        access_token = token.AccessToken,
                        token_type = token.TokenType,
                        user_Id = UserEmail.Id,
                        user_name = UserEmail.Name,
                        expires_in = token.ExpiresIn,
                        creation_Time = token.ValidFrom,
                        expiration_Time = token.ValidTo,
                    });

                }
            }
        }

        }
    }
