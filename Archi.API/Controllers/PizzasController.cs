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

        public PizzasController(ArchiDbContext context):base(context)
        {
        }

        // GET: api/Pizzas?name=Margarita
        //[HttpGet("filters")]
        //public async Task<ActionResult<IEnumerable<Pizza>>> GetFilters(int price, string name)
        //{
        //    var item = await _context.Pizzas.Where(x => x.Price == price).Where(x => x.Name == name).ToListAsync();

        //    return item;
        //}

    }
}
