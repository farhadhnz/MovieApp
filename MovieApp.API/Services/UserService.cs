using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieApp.API.Infrastructure;
using MovieApp.API.Models.Resources;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> userManager;
        private readonly IMapper mapper;
        private readonly AppSettings _appSettings;

        public UserService(UserManager<UserEntity> userManager
            , IMapper mapper
            , IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            _appSettings = appSettings.Value;
        }
        public async Task<User> GetUserAsync(ClaimsPrincipal user)
        {
            var entity = await userManager.GetUserAsync(user);

            return mapper.Map<User>(entity);
        }

        public async Task<User> Authenticate(string userName, string password)
        {
            // TODO
            //var user = await userManager.FindByNameAsync(userName);

            var user = new UserEntity() { UserName = userName };

            if (user == null)
                return null;

            // TODO
            //var passwordChecked = await userManager.CheckPasswordAsync(user, password);

            //if (!passwordChecked)
            //    return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return mapper.Map<User>(user);
        }
    }
}
