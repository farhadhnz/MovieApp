using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApp.Repository
{
    public class MovieAppContext : IdentityDbContext<UserEntity, UserRoleIdentity, Guid>
    {
        public MovieAppContext(DbContextOptions options):
            base(options)
        {

        }
        public DbSet<MovieEntity> Movies { get; set; }

    }
}
