using Archi.API.Data;
using Archi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace TestProject1
{
    public  class MockDbContext : ArchiDbContext
    {
        public MockDbContext(DbContextOptions options):base(options)
        {

        }
        public static MockDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase("dbmok").Options;
            var db = new MockDbContext(options);
            db.Pizzas.Add(new Pizza { ID = 1, Active = true, Name = "Pizza0", Price = 10 });
            db.Pizzas.Add(new Pizza { ID = 2, Active = true, Name = "Pizza2", Price = 15 });
            db.Pizzas.Add(new Pizza { ID = 3, Active = false, Name = "Pizza3", Price = 15 });
            db.Pizzas.Add(new Pizza { ID = 4, Active = true, Name = "Pizza4", Price = 15 });
            db.Pizzas.Add(new Pizza { ID = 5, Active = true, Name = "Pizza5", Price = 15 });
            db.SaveChanges();
            return db;
        }
    }
}

