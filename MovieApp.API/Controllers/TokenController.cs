using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieApp.API.Models.Resources;
using MovieApp.API.Services;
using MovieApp.Repository.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IUserService _userService;
        private readonly RoleManager<UserRoleIdentity> _roleManager;
        private readonly IConfiguration config;

        public TokenController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<UserEntity> signInManager,
            IUserService userService,
            RoleManager<UserRoleIdentity> roleManager,
            IConfiguration config)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userService = userService;
            _roleManager = roleManager;
            this.config = config;
        }

        //[HttpPost(Name = nameof(TokenExchange))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> TokenExchange(OpenIdConnectRequest request)
        //{
        //    if (!request.IsPasswordGrantType())
        //    {
        //        return BadRequest(new OpenIdConnectResponse
        //        {
        //            Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
        //            ErrorDescription = "The specified grant type is not supported."
        //        });
        //    }

        //    var user = await _userManager.FindByNameAsync(request.Username);
        //    if (user == null)
        //    {
        //        return BadRequest(new OpenIdConnectResponse
        //        {
        //            Error = OpenIdConnectConstants.Errors.InvalidGrant,
        //            ErrorDescription = "The username or password is invalid."
        //        });
        //    }

        //    // Ensure the user is allowed to sign in
        //    if (!await _signInManager.CanSignInAsync(user))
        //    {
        //        return BadRequest(new OpenIdConnectResponse
        //        {
        //            Error = OpenIdConnectConstants.Errors.InvalidGrant,
        //            ErrorDescription = "The specified user is not allowed to sign in."
        //        });
        //    }

        //    // Ensure the user is not already locked out
        //    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
        //    {
        //        return BadRequest(new OpenIdConnectResponse
        //        {
        //            Error = OpenIdConnectConstants.Errors.InvalidGrant,
        //            ErrorDescription = "The username or password is invalid."
        //        });
        //    }

        //    // Ensure the password is valid
        //    if (!await _userManager.CheckPasswordAsync(user, request.Password))
        //    {
        //        if (_userManager.SupportsUserLockout)
        //        {
        //            await _userManager.AccessFailedAsync(user);
        //        }

        //        return BadRequest(new OpenIdConnectResponse
        //        {
        //            Error = OpenIdConnectConstants.Errors.InvalidGrant,
        //            ErrorDescription = "The username or password is invalid."
        //        });
        //    }

        //    // Reset the lockout count
        //    if (_userManager.SupportsUserLockout)
        //    {
        //        await _userManager.ResetAccessFailedCountAsync(user);
        //    }

        //    // Look up the user's roles (if any)
        //    var roles = new string[0];
        //    if (_userManager.SupportsUserRole)
        //    {
        //        roles = (await _userManager.GetRolesAsync(user)).ToArray();
        //    }

        //    // Create a new authentication ticket w/ the user identity
        //    var ticket = await CreateTicketAsync(request, user, roles);

        //    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        //}

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]User login)
        {
            IActionResult response;
            var user = await _userService.Authenticate(login.UserName, login.Password);
            
            if (user == null)
                response = BadRequest(new { message = "Username or password is incorrect" });
            else
                response = Ok(user);
            
            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Issuer"],
              null,
              notBefore : null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User AuthenticateUser(User login)
        {
            User user = null;

            //Validate the User Credentials  
            //Demo Purpose, I have Passed HardCoded User Information  
            if (login.FirstName == "Jignesh")
            {
                user = new User { FirstName = "Jignesh", LastName = "Trivedi", Email = "test.btest@gmail.com" };
            }
            return user;
        }

        //private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, UserEntity user, string[] roles)
        //{
        //    var principal = await _signInManager.CreateUserPrincipalAsync(user);

        //    AddRolesToPrincipal(principal, roles);

        //    var ticket = new AuthenticationTicket(principal,
        //        new AuthenticationProperties(),
        //        OpenIdConnectServerDefaults.AuthenticationScheme);

        //    ticket.Principal.SetScopes(OpenIddictConstants.Scopes.Roles);

        //    // Explicitly specify which claims should be included in the access token
        //    foreach (var claim in ticket.Principal.Claims)
        //    {
        //        // Never include the security stamp (it's a secret value)
        //        if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType) continue;

        //        // TODO: If there are any other private/secret claims on the user that should
        //        // not be exposed publicly, handle them here!
        //        // The token is encoded but not encrypted, so it is effectively plaintext.

        //        claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);
        //    }

        //    return ticket;
        //}

        private static void AddRolesToPrincipal(ClaimsPrincipal principal, string[] roles)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var alreadyHasRolesClaim = identity.Claims.Any(c => c.Type == "role");
            if (!alreadyHasRolesClaim && roles.Any())
            {
                identity.AddClaims(roles.Select(r => new Claim("role", r)));
            }

            var newPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
        }
    }
}
