using MovieApp.API.Models.Linking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Models.Response
{
    public class RootResponse : Resource
    {
        public Link Info { get; set; }

        public Link Movies { get; set; }
    }
}
