using MovieApp.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Models.Resources
{
    public class Movie : Resource
    {
        [Sortable]
        [Searchable]
        public string Title { get; set; }

        [Sortable]
        [SearchableDecimal]
        public decimal Rate { get; set; }

    }
}
