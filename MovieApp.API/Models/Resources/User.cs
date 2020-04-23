using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Models.Resources
{
    public class User : Resource
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
