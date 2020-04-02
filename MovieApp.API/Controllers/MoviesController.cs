using Microsoft.AspNetCore.Mvc;
using MovieApp.API.Models;
using MovieApp.API.Models.Linking;
using MovieApp.API.Models.Paging;
using MovieApp.API.Models.Resources;
using MovieApp.API.Models.Searching;
using MovieApp.API.Models.Sorting;
using MovieApp.API.Services;
using MovieApp.Repository;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApp.API.Controllers
{
    [Route("/[controller]")]
    public class MoviesController : Controller
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
        }

        [HttpGet(Name = nameof(GetMoviesAsync))]
        public async Task<IActionResult> GetMoviesAsync(CancellationToken ct
            , [FromQuery] PagingOptions pagingOptions
            , [FromQuery] SortOptions<Movie, MovieEntity> sortOptions
            , [FromQuery] SearchOptions<Movie, MovieEntity> searchOptions)
        {
            var movies = await _service.GetMoviesAsync(searchOptions, sortOptions, pagingOptions, ct);

            var collectionLink = Link.ToCollection(nameof(GetMoviesAsync));

            var collection = PagedCollection<Movie>.Create(collectionLink,
                movies.Items.ToArray(),
                movies.TotalSize,
                pagingOptions);

            return Ok(collection);
        }

        [HttpGet("{movieId}", Name = nameof(GetMovieByIdAsync))]
        public async Task<IActionResult> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
        {
            var movie = await _service.GetMovieByIdAsync(movieId, ct);
            if (movie == null) return NotFound();

            return Ok(movie);
        }
    }
}
