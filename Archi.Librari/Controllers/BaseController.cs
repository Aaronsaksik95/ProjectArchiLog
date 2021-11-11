﻿using Archi.Librari.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archi.Librari.Controllers
{
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : DbContext where TModel : ModelBase
    {
        //Difference entre prive et protected c'est que protected descend dans l'héritage
        protected readonly TContext _context;

        public BaseController(TContext context)
        {
            _context = context;
        }

        // GET: api/Pizzas
<<<<<<< Updated upstream
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll(string range, string asc)
=======
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll(string range, string asc, string desc)
>>>>>>> Stashed changes
        {
            var query = _context.Set<TModel>().Where(x => x.Active == true);
            if (!String.IsNullOrEmpty(range))
            {
                query = Range(range, query);
            }

            if (!String.IsNullOrEmpty(asc))
            {
                return await Sorting(asc, query).ToListAsync();
<<<<<<< Updated upstream
=======
            }
            else if (!String.IsNullOrEmpty(desc))
            {
                return await Sorting(desc, query).ToListAsync();
>>>>>>> Stashed changes
            }
            return await query.ToListAsync();
        }*/

        // GET: api/Pizzas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TModel>> GetId(int id)
        {
            var item = await _context.Set<TModel>().FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        //// PUT: api/Pizzas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutId(int id, TModel model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }
            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/{model}
        [HttpPost]
        public async Task<ActionResult<TModel>> PostModel(TModel tmodel)
        {
            _context.Set<TModel>().Add(tmodel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = tmodel.ID }, tmodel);
        }

        // DELETE: api/{model}/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var item = await _context.Set<TModel>().FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _context.Set<TModel>().Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected IQueryable<TModel> Range(string range, IQueryable<TModel> query)
        {
            string[] num = range.Split("-");
            int num1 = int.Parse(num[0]);
            int num2 = int.Parse(num[1]);
            query = query.Skip(num1).Take(num2);

            return query;
        }

<<<<<<< Updated upstream
        protected IOrderedQueryable<TModel> Sorting(string asc, IQueryable<TModel> query)
        {
            var propertyInfo = typeof(TModel).GetProperty(asc);
            Console.WriteLine(propertyInfo);
            var query2 = query.OrderBy(y => y.GetType().GetProperty(asc, System.Reflection.BindingFlags.IgnoreCase));
            return query2;
        }
=======
        // GET: api/Pizzas?asc=price
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> Sorting(string asc, string desc, IQueryable<TModel> query)
        {
            if(asc != null)
            {

                var source = _context.Set<TModel>();

                // LAMBDA: x => x.[PropertyName]
                var parameter = Expression.Parameter(typeof(TModel), "x");

                Expression property = Expression.Property(parameter, asc);
                var lambda = Expression.Lambda(property, parameter);

                // REFLECTION: source.OrderBy(x => x.Property)
                var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
                var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
                query = (IQueryable<TModel>)orderByGeneric.Invoke(null, new object[] { source, lambda });

                return await ((IOrderedQueryable<TModel>)query).ToListAsync();
            }
            if (desc != null)
            {

                var source = _context.Set<TModel>();

                // LAMBDA: x => x.[PropertyName]
                var parameter = Expression.Parameter(typeof(TModel), "x");

                Expression property = Expression.Property(parameter, desc);
                var lambda = Expression.Lambda(property, parameter);

                // REFLECTION: source.OrderBy(x => x.Property)
                var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
                var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
                query = (IQueryable<TModel>)orderByGeneric.Invoke(null, new object[] { source, lambda });

                return await ((IOrderedQueryable<TModel>)query).ToListAsync();
            }
            else
            {
                return await _context.Set<TModel>().Where(x => x.Active == true).ToListAsync();
            }
        }*/
>>>>>>> Stashed changes

        private bool ModelExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.ID == id);
        }
    }
}                                                