using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Archi.API.Data;
using Archi.API.Models;
using System.Reflection;

namespace Archi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        //injection de dépendance
        private readonly ArchiDbContext _context;

        public CustomersController(ArchiDbContext context)
        {
            _context = context;
        }
        //fin

        // GET: api/Customers
        [HttpGet] //agit appel en lecture
        // méthode asynchrone génère une tâche (= promise en js)
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers() //AcitonResult c'est un résultat, ce qui va en ressortir c'est une tableau de Customer
        {

            return await _context.Customers.Where(x => x.Active == true).ToListAsync(); //await pour faire exécuter et renvoyer ce qu'elle aura traité (sinon c'est task et faut l'exécuté)
        } // Where expression lambda

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id) // on récupère l'id en paramètre
        {
            var customer = await _context.Customers.FindAsync(id); //_context mon archi dbcontext, on peut travailler la base de données depuis le _context
            //_context.Customers.Add()

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }




        /*// GET: api/Customers?range=1-7
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer(Pagination pagination)
        {
            var echantillon = (from customer in await _context.Customers.
                               OrderBy(a => a.Active)
                               select customer).AsQueryable();
            var echantillonbis = echantillon.ToList();
            return echantillonbis;
        }
        */

        // GET: api/Customers?range={debut}-{fin}
        [HttpGet("xxx")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            IQueryable<Customer> query = _context.Customers.AsQueryable().Skip(2); //fonction pas asynchrone donc pas de avait
            return await query.ToListAsync(); //fonction asynchrone donc await
        }


        /*
        // GET: api/Pizzas?name=Margarita
        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<Pizza>>> GetFilters(int price, string name)
        {
            var item = await _context.Pizzas.Where(x => x.Price == price).Where(x => x.Name == name).ToListAsync();

            return item;
        }
        */

        // GET: api/customers?firstname=Lucas
        /*[HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetFilters(string firstname)
        {
            var item = await _context.Customers.Where(x => x.Active == true).Where(x => x.Firstname == firstname).ToListAsync();

            return item;
        }*/


        /*
        // GET: api/customers/filters?lastname=xxxx&firstname=xx
        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetFilters(string lastname, string firstname)
        {
            var item = await _context.Customers.Where(x => x.Active == true).Where(x => x.Lastname == lastname).Where(x => x.Firstname == firstname).ToListAsync();

            return item;
        }
        */

        //CETTE PARTIE LA A REGARDER
        //GET: api/customers/filters?Email=string
        //+e=xxxx,lucas&Lastname=xx
        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetFiltersValeur()
        {
            IQueryable<Customer> query = _context.Customers.Where(x => x.Active == true); //fonction pas asynchrone, récupère la table de Customers
            foreach (var item in Request.Query) //parcourt l'url (dans l'url après ?, item prend chaque morceau séparé par &)
            {
                Type propbis = typeof(Customer);
                /*var property2 = propbis.GetProperties();
                foreach (var propInf in property2)
                {
                    var xxx = propInf.Name;
                    
                    if (propInf.Name == item.Key)
                    {
                        var valeur = item.Value;
                        query = query.Where(x => propInf.GetValue(x) == valeur); //fonctionne pas car propInf.Name (à la place de propInf) n'est pas de type PropertyInfo
                        break;
                    }
                    else
                    {
                        return NotFound();
                    }
                    
                    
                }*/
                //Fonctionne pas car prop est null alors qu'il devrait être égale à firstname
                //Type propbis = typeof(Customer);
                var prop = propbis.GetProperty(item.Key, System.Reflection.BindingFlags.IgnoreCase); //le type de customer . getProperty va chercher une propriété soit la clé de item en ignorant majuscule et minuscule
                //GetProperty recherche la propriété spécifiée, à l'aide des contraintes de liaison spécifiées.
                if (prop != null) // si la clé et le champ de Customer se correspondent
                {
                    var valeur = item.Value; // va chercher la valeur dans l'url (soit toto ici)
                    query = query.Where(x => prop.GetValue(x) == valeur); //on passe par prop et la condition est la valeur de prop (soit la valeur du champ de prop de la table Customer) == valeur (soit la valeur de l'url)
                    // GetValue retourne la valeur de la propriété d'un objet spécifié.
                }
                else
                {
                    return NotFound();
                }
            }
            return await query.ToListAsync();
        }


    /*
    //GET: api/customers/filters?lastname=xxxx,lucas
    [HttpGet("filters")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetFiltersValeur(string lastname)
    {
        string[] val = lastname.Split(",");
        var item = await _context.Customers.Where(x => x.Active == true).Where(x => x.Lastname == val[0] || x.Lastname == val[1]).ToListAsync();
        return item;
    }
    */



    /*
    //GET: api/customers/filters?name=aaron,lucas,leo
    [HttpGet("filters")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetFiltersValeur(string firstname, string lastname, string email)
    {
        string[] val = firstname.Split(",");
        var query = _context.Customers.Where(x => x.Active == true);

        for (int i = 0; i < firstname.Length; i++)
        {
            query = query.Where(x => x.Firstname == val[i]);
        }
        return await query.ToListAsync();
    }
    */



    /*
    //GET: api/customers/filters?lastname=xxxx&firstname=xx
    [HttpGet("filterss")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetFilters(string valeur)
    {
        string[] val = valeur.Split(",");
        var item = _context.Customers.Where(x => x.Active == true).AsQueryable;
        for (int i=0 ; i<valeur.Length ; i++)
        {
            item = item.Where(x => x.Lastname == val[i]).ToListAsync();
        }
        return item;

    }
    */

    /*
     * public async Task<ActionResult<IEnumerable<Customer>>> GetFiltersValeur(float[] tab, float val)
     * {
     *    for (int i = 0; i < tab.Length; ++i)
          {
              tab[i] = val;
          }
     * }
     * */

    /*
    public async Task<ActionResult<IEnumerable<TModel>>> Range(string range)
    {
        Console.WriteLine(Request.QueryString);
        string[] num = range.Split("-");
        int num1 = int.Parse(num[0]);
        int num2 = int.Parse(num[1]);
        var query = await _context.Set<TModel>().Where(x => x.Active == true).Skip(num1).Take(num2).ToListAsync();

        return query;
    }
    */

    //type=pizza,pates&rating=4,5&days=sunday








    // PUT: api/Customers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer) // il va regarder le body si la structure json est la même structure que le customer
        {
            if (id != customer.ID)
            {
                return BadRequest(); // code 400 (403 : forbidden -> requête interdite ; 404 : route pas trouvé, 401 : pas authentifié ; 500 : erreur d'exécuter serveur ; 200 : c'est ok ; 201 : created -> la ressource a bien été créée)
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            { // SaveChanges fait le commit et réouvre une transaction, si problème rollback
                await _context.SaveChangesAsync(); // c'est à ce moment là qu'il fait le commit (transaction) (verrou enlevé) (valider l'intégration des données là où le commit est), soit un retour tout bon ou si une requête fonctionne pas le retour est vide
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.ID }, customer);
        }

        // DELETE: api/Customers/5
        // On modifie le delete pour flaguer/historiser
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            //customer.Active = false;
            //_context.Entry(customer).State = EntityState.Modified;// indique à l'orm qu'on a modifié un objet, passe l'état de l'objet en modifié, fait un update
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(); //c'est une méthode virtual dans la classe dbContext, virtual c'est à dire a déjà sa fonctionnalité mais on doit la réécrire lors de son héritage, on autorise une redéfinition (final en java bloque la redéfinition car c'est l'inverse elles sont toutes redéfinies de base)
            // pour redéfinir une méthode virtual on l'override

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.ID == id);
        }
    }
}



