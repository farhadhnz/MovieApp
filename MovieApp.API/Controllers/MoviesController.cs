using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Controllers
{
    [Route("/[controller]")]
    public class MoviesController : Controller
    {
        [HttpGet(Name = nameof(GetMovies))]
        public IActionResult GetMovies()
        {
            throw new NotImplementedException();
        }
    }
}
