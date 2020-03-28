using Microsoft.AspNetCore.Mvc;
using MovieApp.API.Models.Resources;
using MovieApp.API.Services;
using MovieApp.Repository;
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

        [HttpGet(Name = nameof(GetMovies))]
        public IActionResult GetMovies()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{movieId}", Name = nameof(GetMovieByIdAsync))]
        public async Task<IActionResult> GetMovieByIdAsync(Guid movieId, CancellationToken ct)
        {
            var movie = await _service.GetMovieByIdAsync(movieId, ct);
            if (movie == null) return NotFound();

            //var resource = new Movie
            //{
            //    Href = Url.Link(nameof(GetMovieByIdAsync), new { roomId = movie.Id }),
            //    Title = entity.Title
            //};

            return Ok(movie);
        }
    }
}
