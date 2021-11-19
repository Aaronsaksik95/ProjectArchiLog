using Archi.Librari.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Archi.Librari.Controllers
{
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : DbContext where TModel : ModelBase
    {
        protected readonly TContext _context;

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

                else
                {
                    if (value.StartsWith("["))
                    {
                        query = FilteringWithInterval(key, value, query);
                    }
                    else
                    {
                        query = Filtering(key, value, query);
                    }
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
                    if (value.StartsWith("["))
                    {
                        query = FilteringWithInterval(key, value, query);
                    }
                    else
                    {
                        query = Filtering(key, value, query);
                    }
                    
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

            int gap = num2 - num1;
            var queryCount = query.Count();

            var Schema = Request.Scheme;
            var Host = Request.Host;

            query = query.Skip(num1).Take(gap+1);

            Type isType = typeof(TModel);
            string path = Request.Path;
            string typeName = isType.Name;
            Response.Headers.Add("Content-Range", range + "/" + queryCount);
            Response.Headers.Add("Accept-Range", typeName + " " + gap);

            string firstPart = Schema + "://" + Host + path + "?range=0-" + num1;
            string prevPart = Schema + "://" + Host + path + "?range="+ ((num1 - 1) - gap)+ "-" + (num1 - 1);
            string nextPart = Schema + "://" + Host + path + "?range=" + (num2 +1) + "-" + ((num2 + 1) + gap);
            string lastPart = Schema + "://" + Host + path + "?range=" + (queryCount - gap) + "-" + queryCount;

            Response.Headers.Add("first", firstPart);
            Response.Headers.Add("prev", prevPart);
            Response.Headers.Add("next", nextPart);
            Response.Headers.Add("last", lastPart);



            return query;
        }

        protected IQueryable<TModel> Ascending(string asc, IQueryable<TModel> query)
        {
            Expression property = Expression.Property(parameter, asc);
            var lambda = Expression.Lambda(property, parameter);

            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { query, lambda });

            return ((IOrderedQueryable<TModel>)result);
        }

        protected IQueryable<TModel> Descending(string desc, IQueryable<TModel> query)
        {   
            Expression property = Expression.Property(parameter, desc);
            var lambda = Expression.Lambda(property, parameter);
            
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TModel), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { query, lambda });
            
            return ((IOrderedQueryable<TModel>)result);
        }

        protected IQueryable<TModel> Filtering(string key, string value, IQueryable<TModel> query)
        {
            string[] valueSplit = value.Split(",");

            Expression property = Expression.Property(parameter, key);
            Expression<Func<TModel, bool>> lambda;
            List<Expression> listExp = new List<Expression>();

            foreach (var itemValue in valueSplit)
            {
                ConstantExpression constantExp;
                if (Nullable.GetUnderlyingType(property.Type) != null)
                {
                    object valueCast = Convert.ChangeType(itemValue, Nullable.GetUnderlyingType(property.Type));
                    constantExp = Expression.Constant(valueCast);
                }
                else
                {
                    constantExp = Expression.Constant(itemValue);
                }

                var convert = Expression.Convert(constantExp, property.Type);
                var equals = (Expression)Expression.Equal(property, convert);
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

            query = query.Where(lambda);
            
            return query;
        }

        protected IQueryable<TModel> FilteringWithInterval(string key, string value, IQueryable<TModel> query)
        {
            string[] valueSplit = value.Split(",");

            Expression property = Expression.Property(parameter, key);
            Expression<Func<TModel, bool>> lambda;

            var lessValue = string.Join("", valueSplit[0].Split('['));
            var greaterValue = string.Join("", valueSplit[1].Split(']'));

            if (lessValue != "" && greaterValue == "")
            {
                object lessValueCast = Convert.ChangeType(lessValue, Nullable.GetUnderlyingType(property.Type));
                var constantLess = Expression.Constant(lessValueCast);
                var convertLess = Expression.Convert(constantLess, property.Type);
                var before = Expression.GreaterThanOrEqual(property, convertLess);
                lambda = Expression.Lambda<Func<TModel, bool>>(before, parameter);
            }
            else if (greaterValue != "" && lessValue == "")
            {
                object greaterValueCast = Convert.ChangeType(greaterValue, Nullable.GetUnderlyingType(property.Type));
                var constantGreater = Expression.Constant(greaterValueCast);
                var convertGreater = Expression.Convert(constantGreater, property.Type);
                var after = Expression.LessThanOrEqual(property, convertGreater);
                lambda = Expression.Lambda<Func<TModel, bool>>(after, parameter);
            }
            else
            {
                object lessValueCast = Convert.ChangeType(lessValue, Nullable.GetUnderlyingType(property.Type));
                var constantLess = Expression.Constant(lessValueCast);
                var convertLess = Expression.Convert(constantLess, property.Type);
                var before = Expression.GreaterThanOrEqual(property, convertLess);

                object greaterValueCast = Convert.ChangeType(greaterValue, Nullable.GetUnderlyingType(property.Type));
                var constantGreater = Expression.Constant(greaterValueCast);
                var convertGreater = Expression.Convert(constantGreater, property.Type);
                var after = Expression.LessThanOrEqual(property, convertGreater);

                var bothExp = (Expression)Expression.And(before, after);
                lambda = Expression.Lambda<Func<TModel, bool>>(bothExp, parameter);
            }

            query = query.Where(lambda);

            return query;
        }

        protected IQueryable<TModel> Search(string key, string value, IQueryable<TModel> query)
        {
            Expression property = Expression.Property(parameter, key);
            Expression<Func<TModel, bool>> lambda;

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
