using MovieApp.API.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieApp.API.Services
{
    public interface IUserService
    {
        //Task<User> GetUserAsync()
        Task<User> GetUserAsync(ClaimsPrincipal user);
        Task<User> Authenticate(string userName, string password);
    }
}
