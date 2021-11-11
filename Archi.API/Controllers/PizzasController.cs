using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Archi.API.Data;
using Archi.API.Models;
using Archi.Librari.Controllers;
using System.Text.RegularExpressions;

namespace Archi.API.Controllers
{

    public static class MyStringExtensions
    {
        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : BaseController<ArchiDbContext, Pizza>
    {

        public PizzasController(ArchiDbContext context):base(context)
        {
        }

        // GET: api/Pizzas?name=Margarita
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pizza>>> SearchAsync(string name)
        {
            var query = await _context.Pizzas.Where(x => x.Name == x.Like(name)).ToListAsync();

            return query;
        }

    }
}
