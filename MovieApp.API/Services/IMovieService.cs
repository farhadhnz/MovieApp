using MovieApp.API.Models.Paging;
using MovieApp.API.Models.Resources;
using MovieApp.API.Models.Searching;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.API.Services
{
    public interface IMovieService
    {
        Task<Movie> GetMovieByIdAsync(Guid movieId, CancellationToken ct);
        Task<PagedResults<Movie>> GetMoviesAsync(SearchOptions<Movie, MovieEntity> searchOptions, Models.Sorting.SortOptions<Movie, MovieEntity> sortOptions, PagingOptions pagingOptions, CancellationToken ct);
    }
}
