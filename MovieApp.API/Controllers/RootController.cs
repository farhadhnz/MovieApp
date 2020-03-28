using Microsoft.AspNetCore.Mvc;
using MovieApp.API.Models.Linking;
using MovieApp.API.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Controllers
{
    [Route("/")]
    [ApiVersion("1.0")]
    public class RootController : Controller
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new RootResponse
            {
                //href = Url.Link(nameof(GetRoot), null),
                Movies = Link.To(nameof(MoviesController.GetMoviesAsync))
            };

            return Ok(response);
        }
    }
}
