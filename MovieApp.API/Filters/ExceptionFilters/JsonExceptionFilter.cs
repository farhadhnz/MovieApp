using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.API.Models.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Filters.ExceptionFilters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;

        public JsonExceptionFilter(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();

            if (env.EnvironmentName == "Development")
            {
                error.Message = context.Exception.Message;
                error.Details = context.Exception.StackTrace;
            }
            else
            {
                error.Message = "A server error occured.";
                error.Details = context.Exception.Message;
            }

            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}
