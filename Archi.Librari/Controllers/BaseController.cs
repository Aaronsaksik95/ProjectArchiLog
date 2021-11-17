using Archi.Librari.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Archi.Librari.Controllers
{
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : DbContext where TModel : ModelBase
    {
        protected readonly TContext _context;

        private Expression<Func<TModel, bool>> lambda;

        // LAMBDA: x => x.[PropertyName]
        private ParameterExpression parameter = Expression.Parameter(typeof(TModel), "x");

        public BaseController(TContext context)
        {
            _context = context;
        }

        // GET: api/Pizzas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAll()
        {
            var query = _context.Set<TModel>().Where(x => x.Active == true);
            
            foreach (String key in Request.Query.Keys)
            {
                var value = Request.Query[key];
                if (key == "range")
                {
                    query = Range(value, query);
                }

                else if (key == "asc")
                {
                    query = Ascending(value, query);
                }

                else if (key == "desc")
                {
                    query = Descending(value, query);
                }

                else
                {
                    query = Filtering(key, value, query);
                }
            }

            return await query.ToListAsync();
        }

        // GET: api/Pizzas/search?name=*san*
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TModel>>> GetSearch()
        {
            var query = _context.Set<TModel>().Where(x => x.Active == true);

            foreach (String key in Request.Query.Keys)
            {
                String value = Request.Query[key];
                if (key == "range")
                {
                    query = Range(value, query);
                }

                else if (key == "asc")
                {
                    query = Ascending(value, query);
                }

                else if (key == "desc")
                {
                    query = Descending(value, query);
                }

                else if (value.StartsWith("*"))
                {
                    query = Search(key, value, query);
                }

                else
                {
                    query = Filtering(key, value, query);
                }
            }

            return await query.ToListAsync();
        }

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

        protected IQueryable<TModel> Ascending(string asc, IQueryable<TModel> query)
        {
            Expression property = Expression.Property(parameter, asc);
            var lambda = Expression.Lambda(property, parameter);

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { query, lambda });

            return ((IOrderedQueryable<TModel>)result);
        }

        protected IQueryable<TModel> Descending(string desc, IQueryable<TModel> query)
        {
            // LAMBDA: x => x.[PropertyName]
            
            Expression property = Expression.Property(parameter, desc);
            var lambda = Expression.Lambda(property, parameter);
            
            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { query, lambda });

            return ((IOrderedQueryable<TModel>)result);
        }

        protected IQueryable<TModel> Filtering(string key, string value, IQueryable<TModel> query)
        {
            string[] valueSplit = value.Split(",");
            List<Expression> listExp = new List<Expression>();

            Expression property = Expression.Property(parameter, key);

            //var propertyType = ((PropertyInfo)property).PropertyType;
            //var converter = TypeDescriptor.GetConverter(propertyType);
            foreach (var itemValue in valueSplit)
            {
                //var num = int.Parse(itemValue);

                var constantExp = Expression.Constant(itemValue, typeof(string));
                var equals = (Expression)Expression.Equal(property, constantExp);
                listExp.Add(equals);
            }
            Expression[] arrayExp = listExp.ToArray();

            if (arrayExp.Length > 1)
            {
                var bothExp = (Expression)Expression.Or(arrayExp[0], arrayExp[1]);
                lambda = Expression.Lambda<Func<TModel, bool>>(bothExp, parameter);
            }

            else
            {
                lambda = Expression.Lambda<Func<TModel, bool>>(arrayExp[0], parameter);
            }

            //Where(x => x.Name == "olive" || x => x.Name == "margarita")
            query = query.Where(lambda);

            return query;
        }

        protected IQueryable<TModel> Search(string key, string value, IQueryable<TModel> query)
        {
            Expression property = Expression.Property(parameter, key);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(string.Join("", value.Split('*')), typeof(string));
            var containsMethodExp = Expression.Call(property, method, someValue);
            lambda = Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parameter);
            query = query.Where(lambda);
            return query;
        }

        private bool ModelExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.ID == id);
        }
    }
}