using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApp.Repository.Models
{
    public class UserRoleIdentity : IdentityRole<Guid>
    {
        public UserRoleIdentity()
        : base()
        {

        }

        public UserRoleIdentity(string roleName)
            : base(roleName)
        {

        }
    }
}
