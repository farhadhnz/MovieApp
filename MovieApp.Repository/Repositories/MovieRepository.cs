using Microsoft.EntityFrameworkCore;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.Repository.Repositories
{
    public class MovieRepository
    {
        private readonly MovieAppContext _context;

        public MovieRepository(MovieAppContext context)
        {
            _context = context;
        }

        public async Task<MovieEntity> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
        {
            var entity = await _context.Movies.SingleOrDefaultAsync(m => m.Id == movieId, ct);

            return entity;
        }

        public IQueryable<MovieEntity> GetMoviesAsync(CancellationToken ct)
        {
            var entities = _context.Movies.AsQueryable();

            return entities;
        }
    }
}
