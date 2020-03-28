using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieApp.API.Models.Paging;
using MovieApp.API.Models.Resources;
using MovieApp.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.API.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieRepository repository;
        private readonly IMapper mapper;

        public MovieService(MovieRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Movie> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
        {
            var movieEntity = await repository.GetMovieByIdAsync(movieId, ct);

            return mapper.Map<Movie>(movieEntity);
        }

        public async Task<PagedResults<Movie>> GetMoviesAsync(PagingOptions pagingOptions, CancellationToken ct)
        {
            var moviesQueryable = repository.GetMoviesAsync(ct);

            var query = mapper.ProjectTo<Movie>(moviesQueryable);

            var results = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ToArrayAsync();

            var totalSize = await query.CountAsync();

            return new PagedResults<Movie>
            {
                Items = results,
                TotalSize = totalSize
            };
        }
    }
}
