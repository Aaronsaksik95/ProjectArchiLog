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

namespace Archi.API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : BaseController<ArchiDbContext, Pizza>
    {

        public PizzasController(ArchiDbContext context) : base(context)
        {
        }


        // GET: api/Pizzas/search?name=san
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pizza>>> Search(string name)
        {
            var results = await _context.Pizzas.Where(X => X.Name.Contains(name) == true).ToListAsync();
            return results;
        }

    }
}
