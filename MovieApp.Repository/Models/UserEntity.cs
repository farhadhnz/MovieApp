using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApp.Repository.Models
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Token { get; set; }
    }
}
